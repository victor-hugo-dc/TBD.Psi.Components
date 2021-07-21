using Microsoft.Psi;
using Microsoft.Psi.Imaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;

namespace TBD.Psi.RosBagStreamReader.Deserializers
{
    public class SensorMsgsCompressedImageAsDepthImageDeserializer : MsgDeserializer
    {
        public SensorMsgsCompressedImageAsDepthImageDeserializer(bool useHeaderTime)
            : base(typeof(Shared<DepthImage>).AssemblyQualifiedName, "sensor_msgs/CompressedImage", useHeaderTime)
        {
        }

        public override T Deserialize<T>(byte[] data, ref Envelope env)
        {
            // read the header and get location
            (_, var originTime, _) = Helper.ReadStdMsgsHeader(data, out var offset, 0);
            this.UpdateEnvelope(ref env, originTime);

            var formatStrLength = (int)BitConverter.ToUInt32(data, offset);
            var format = Encoding.UTF8.GetString(data, offset + 4, formatStrLength).ToLower();

            var dataArr = data.Skip(offset + formatStrLength + 4 + 4).ToArray();
            var imageMemoryStream = new MemoryStream(dataArr);
            if (format.Contains("tiff"))
            {
                var decodedImg = new TiffBitmapDecoder(imageMemoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decodedImg.Frames[0];
                using (var sharedDepthImage = EncodedImagePool.GetOrCreate(bitmapSource.PixelWidth, bitmapSource.PixelHeight, PixelFormat.BGRA_32bpp))
                {
                    bitmapSource.CopyPixels(Int32Rect.Empty, sharedDepthImage.Resource.ImageData,
                                            sharedDepthImage.Resource.Stride * sharedDepthImage.Resource.Height,
                                            sharedDepthImage.Resource.Stride);
                    return (T)(object)sharedDepthImage.AddRef();
                }
            }
            else if (format == "png")
            {
                var decodedImg = new PngBitmapDecoder(imageMemoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decodedImg.Frames[0];
                using (var sharedDepthImage = DepthImagePool.GetOrCreate(bitmapSource.PixelWidth, bitmapSource.PixelHeight))
                {
                    bitmapSource.CopyPixels(Int32Rect.Empty, sharedDepthImage.Resource.ImageData,
                                            sharedDepthImage.Resource.Stride * sharedDepthImage.Resource.Height,
                                            sharedDepthImage.Resource.Stride);
                    return (T)(object)sharedDepthImage.AddRef();
                }
                new EncodedDepthImage()
            }
            throw new NotSupportedException($"{format} is not supported.");
        }
    }
}

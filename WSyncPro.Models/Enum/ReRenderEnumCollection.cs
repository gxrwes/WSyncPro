using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace WSyncPro.Models.Enum.ReRenderEnumCollection
{
    public class VideoTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return VideoType.All.FirstOrDefault(vt => vt.FileExtension == stringValue) ??
                       throw new InvalidOperationException($"Unknown VideoType: {stringValue}");
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is VideoType videoType)
            {
                return videoType.FileExtension;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(VideoTypeConverter))]
    [Serializable]
    public class VideoType
    {
        public string FileExtension { get; private set; }
        public string CodecName { get; private set; }

        private VideoType(string fileExtension, string codecName)
        {
            FileExtension = fileExtension;
            CodecName = codecName;
        }

        public static readonly VideoType MP4 = new VideoType(".mp4", "H.264");
        public static readonly VideoType MKV = new VideoType(".mkv", "H.265");
        public static readonly VideoType AVI = new VideoType(".avi", "DivX");
        public static readonly VideoType MOV = new VideoType(".mov", "ProRes");
        public static readonly VideoType WEBM = new VideoType(".webm", "VP9");
        public static readonly VideoType FLV = new VideoType(".flv", "H.263");

        public static IEnumerable<VideoType> All => new[] { MP4, MKV, AVI, MOV, WEBM, FLV };

        public override string ToString() => $"{CodecName} ({FileExtension})";

        public override bool Equals(object obj)
        {
            if (obj is VideoType other)
                return FileExtension == other.FileExtension && CodecName == other.CodecName;
            return false;
        }

        public override int GetHashCode() => (FileExtension, CodecName).GetHashCode();
    }
}

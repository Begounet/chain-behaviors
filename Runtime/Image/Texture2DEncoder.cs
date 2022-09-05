using AUE;
using ChainBehaviors.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChainBehaviors.Image
{
    /// <summary>
    /// Encode a Texture2D as byte[] (jpg, png etc.)
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleImagePath + "Texture2D Encoder")]
    public class Texture2DEncoder : BaseMethod
    {
        public enum EFormat
        {
            PNG,
            JPG,
            TGA,
            EXR
        }

        [SerializeField]
        private EFormat _format = EFormat.PNG;
        public EFormat Format { get => _format; set => _format = value; }

        [SerializeField, ShowIf(nameof(_format), EFormat.JPG)]
        private int _jpgQuality = 90;
        public int JPGQuality
        {
            get => _jpgQuality;
            set => _jpgQuality = value;
        }

        [SerializeField, ShowIf(nameof(_format), EFormat.EXR)]
        private Texture2D.EXRFlags _EXRFlags = Texture2D.EXRFlags.CompressRLE;
        public Texture2D.EXRFlags EXRFlags { get => _EXRFlags; set => _EXRFlags = value; }

        [SerializeField]
        private AUEEvent<byte[]> _encoded;
        public AUEEvent<byte[]> Encoded { get => _encoded; set => _encoded = value; }


        public void Export(Texture2D texture)
        {
            byte[] data;
            switch (_format)
            {
                case EFormat.PNG:
                    data = texture.EncodeToPNG();
                    break;
                case EFormat.JPG:
                    data = texture.EncodeToJPG(_jpgQuality);
                    break;
                case EFormat.TGA:
                    data = texture.EncodeToTGA();
                    break;
                case EFormat.EXR:
                    data = texture.EncodeToEXR(_EXRFlags);
                    break;

                default:
                    throw new System.Exception("Unsupported export format");
            }

            Trace(("format", _format));
            _encoded.Invoke(data);
        }
    }
}
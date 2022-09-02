using AUE;
using UnityEngine;

namespace ChainBehaviors
{
    [AddComponentMenu(CBConstants.ModuleImagePath + nameof(Texture2DToSprite))]
    public class Texture2DToSprite : BaseMethod
    {
        [SerializeField]
        private Vector2 _pivot = new Vector2(0.5f, 0.5f);
        public Vector2 Pivot { get => _pivot; set => _pivot = value; }

        [SerializeField]
        private float _pixelsPerUnit = 100;
        public float PixelsPerUnit { get => _pixelsPerUnit; set => _pixelsPerUnit = value; }

        [SerializeField]
        private int _extrude = 1;
        public int Extrude { get => _extrude; set => _extrude = value; }

        [SerializeField]
        private SpriteMeshType _spriteMeshType = SpriteMeshType.Tight;
        public SpriteMeshType SpriteMeshType { get => _spriteMeshType; set => _spriteMeshType = value; }

        [SerializeField]
        private Vector4 _borders = Vector4.zero;
        public Vector4 Borders { get => _borders; set => _borders = value; }

        [SerializeField]
        private bool _generateFallbackPhysicsShape = true;
        public bool GenerateFallbackPhysicsShape { get => _generateFallbackPhysicsShape; set => _generateFallbackPhysicsShape = value; }

        [SerializeField]
        private AUEEvent<Sprite> _converted;


        public void Convert(Texture2D source)
        {
            Sprite newSprite = Sprite.Create(
                source, 
                new Rect(0, 0, source.width, source.height), 
                _pivot, 
                _pixelsPerUnit, 
                (uint) _extrude, 
                _spriteMeshType, 
                _borders, 
                _generateFallbackPhysicsShape);

            Trace(("source", source.name), ("width", source.width), ("height", source.height));
            _converted.Invoke(newSprite);
        }
    }
}
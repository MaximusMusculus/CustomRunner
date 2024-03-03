using Core;

namespace TestTimeProvider
{
    public class TestTimeProvider : ITimeProvider
    {
        private float _deltaTime;
        public float DeltaTime => _deltaTime;

        public void SetDeltaTime(float deltaTime)
        {
            _deltaTime = deltaTime;
        }
    }
}

namespace Game.Conditions.Implementations
{
    /// <summary>
    /// Простой таймер для проверки времени
    /// IResetable?
    /// </summary>
    public class CheckElapsedTime : CheckCondition
    {
        private readonly float _time;
        private float _elapsedTime;
        
        public CheckElapsedTime(float time)
        {
            _elapsedTime = 0f;
            _time = time;
        }
        public override void Update(float deltaTime)
        {
            _elapsedTime += deltaTime;
        }

        public override void Reset()
        {
            _elapsedTime = 0;
        }

        public override bool Check()
        {
            return _elapsedTime >= _time;
        }
    }
}
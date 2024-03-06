using System;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // RegisterMessagePipe returns options.
        var options = builder.RegisterMessagePipe(/* configure option */);
        
        // Setup GlobalMessagePipe to enable diagnostics window and global function
        builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

        // RegisterMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.
        builder.RegisterMessageBroker<int>(options);
        builder.RegisterMessageBroker<IGameEvent>(options);


        // also exists RegisterMessageBroker<TKey, TMessage>, RegisterRequestHandler, RegisterAsyncRequestHandler

        // RegisterMessageHandlerFilter: Register for filter, also exists RegisterAsyncMessageHandlerFilter, Register(Async)RequestHandlerFilter
       //builder.RegisterMessageHandlerFilter<MyFilter<int>>();

       //builder.RegisterEntryPoint<MessagePipeSubscriber>();
       //builder.RegisterEntryPoint<MessagePipePublisher>();
       //builder.RegisterEntryPoint<MessagePipeDemo>();
       
       //builder.RegisterEntryPoint<MessagePipeSubscriber2>();

       
       
       //builder.RegisterEntryPoint<BattleEventListener>();
       builder.RegisterEntryPoint<GameEventsListener>();
       builder.RegisterEntryPoint<EventPublisher>();
    }
    
    public interface IGameEvent { }
    public class BattleEvent : IGameEvent
    {
        public int BattleId { get; }
        public BattleEvent(int battleId)
        {
            BattleId = battleId;
        }
    }
    public class CharacterEvent : IGameEvent
    {
        public int CharacterId { get; }
        public CharacterEvent(int characterId)
        {
            CharacterId = characterId;
        }
    }
    
    public class EventPublisher : IStartable
    {
        private readonly IPublisher<IGameEvent> _publisher;
        public EventPublisher(IPublisher<IGameEvent> publisher)
        {
            _publisher = publisher;
        }

        public void Start()
        {
            _publisher.Publish(new BattleEvent(1));
            _publisher.Publish(new CharacterEvent(2));
        }
    }

    public class GameEventsListener : IStartable
    {
        private readonly ISubscriber<IGameEvent> _subscriber;

        public GameEventsListener(ISubscriber<IGameEvent> subscriber)
        {
            _subscriber = subscriber;
        }

        public void Start()
        {
            _subscriber.Subscribe(Handler);
        }

        private void Handler(IGameEvent obj)
        {
            Debug.Log("On game event: " + obj);
        }
    }
    public class BattleEventListener : IStartable
    {
        private readonly ISubscriber<BattleEvent> _subscriber;

        public BattleEventListener(ISubscriber<BattleEvent> subscriber)
        {
            _subscriber = subscriber;
        }

        public void Start()
        {
            _subscriber.Subscribe(Handler);
        }

        private void Handler(BattleEvent obj)
        {
            Debug.Log("On battle event: " + obj);
        }
    }




    public class MessagePipeSubscriber : IStartable, IDisposable
    {
        readonly ISubscriber<BattleEvent> _subscriber;
        private IDisposable _disposable;

        public MessagePipeSubscriber(ISubscriber<BattleEvent> subscriber)
        {
            _subscriber = subscriber;
        }

        public void Start()
        {
            var d = DisposableBag.CreateBuilder();
            _subscriber.Subscribe(i => Debug.Log("s val" + i)).AddTo(d);
            _disposable = d.Build();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
    public class MessagePipePublisher : IStartable
    {
        readonly IPublisher<int> publisher;

        public MessagePipePublisher(IPublisher<int> publisher)
        {
            this.publisher = publisher;
        }

        public void Start()
        {
            publisher.Publish(10);
            publisher.Publish(20);
            publisher.Publish(30);
        }
    }
    public class MessagePipeDemo : IStartable, IDisposable
    {
        readonly IPublisher<int> publisher;
        readonly ISubscriber<int> subscriber;
        private IDisposable _disposable;

        public MessagePipeDemo(IPublisher<int> publisher, ISubscriber<int> subscriber)
        {
            this.publisher = publisher;
            this.subscriber = subscriber;
        }

        public void Start()
        {
            var d = DisposableBag.CreateBuilder();
            subscriber.Subscribe(x => Debug.Log("S1:" + x)).AddTo(d);
            subscriber.Subscribe(x => Debug.Log("S2:" + x)).AddTo(d);

            publisher.Publish(10);
            publisher.Publish(20);
            publisher.Publish(30);

            _disposable = d.Build();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
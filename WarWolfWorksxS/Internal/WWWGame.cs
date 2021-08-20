using Stride.Engine;
using Stride.Input;
using System;
using System.Threading.Tasks;

namespace WarWolfWorksxS.Internal
{
    /// <summary>
    /// The WarWolfWorks game class.
    /// </summary>
    public class WWWGame : Game
    {
        /// <summary>
        /// Called every physics frame; Unlike relying on <see cref="IsFixedStep"/> and/or <see cref="AwaitFixedStep"/>,
        /// this can be invoked multiple times in the same frame.
        /// </summary>
        public event Action OnFixedUpdate;

        /// <summary>
        /// Is the current frame a physics frame?
        /// </summary>
        public bool IsFixedStep { get; private set; }

        /// <summary>
        /// The delta time of the current frame.
        /// </summary>
        public float DeltaTime { get; private set; }

        /// <summary>
        /// Returns a task which awaits a fixed step frame.
        /// </summary>
        /// <returns></returns>
        public Func<Task> AwaitFixedStep() => () =>
            {
                while(true)
                {
                    if (IsFixedStep)
                        break;
                }

                return Task.CompletedTask;
            };

        /// <summary>
        /// Sets the <see cref="IsFixedStep"/> as well as all delta time values. Make sure to include "base.RawTickProducer();" when overriding.
        /// </summary>
        protected override void RawTickProducer()
        {
            base.RawTickProducer();

            DeltaTime = (float)UpdateTime.Elapsed.TotalSeconds;
            pv_AccumulatedTime -= DeltaTime;
            if (pv_AccumulatedTime <= 0)
            {
                IsFixedStep = true;
                while(pv_AccumulatedTime <= 0)
                {
                    pv_AccumulatedTime += TargetElapsedTime.TotalSeconds;
                    OnFixedUpdate?.Invoke();
                }
            }
            else
            {
                IsFixedStep = false;
            }

        }

        /// <summary>
        /// Sets up the library. When overriding, make sure to include "base.BeginRun();".
        /// </summary>
        protected override void BeginRun()
        {
            base.BeginRun();

            VirtualButtonConfig[] buttons = DefaultVirtualButtons();
            if(buttons != null)
            {
                Input.VirtualButtonConfigSet = Input.VirtualButtonConfigSet ?? new VirtualButtonConfigSet();
                foreach (VirtualButtonConfig config in buttons)
                    Input.VirtualButtonConfigSet.Add(config);
            }

            Debug.Init();
        }

        /// <summary>
        /// Adds this list of configs inside the base <see cref="BeginRun"/> method.
        /// </summary>
        /// <returns></returns>
        protected virtual VirtualButtonConfig[] DefaultVirtualButtons() => null;

        private double pv_AccumulatedTime;
    }
}

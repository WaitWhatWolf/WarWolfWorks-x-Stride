using Stride.Engine;
using System;
using System.Threading.Tasks;
using WarWolfWorksxS.NyuEntities;

namespace WarWolfWorksxS.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on an <see cref="INyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has Task to execute.
    /// </summary>
    public interface INyuTaskExecutable
    {
        /// <summary>
        /// The task to be run.
        /// </summary>
        Func<Task> Task { get; }
        /// <summary>
        /// Determines whether the <see cref="Task"/> is currently running.
        /// </summary>
        bool TaskRunning { get; set; }
        /// <summary>
        /// Reference to the <see cref="AsyncScript"/> which is currently running the <see cref="Task"/>.
        /// </summary>
        AsyncScript Script { get; set; }

        /// <summary>
        /// Call this to set the <see cref="Task"/> to be run on an <see cref="AsyncScript"/>'s micro thread.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="to"></param>
        public void SetTaskRunning(AsyncScript script, bool to)
        {
            if (Script != null && script != Script)
                throw new InvalidOperationException("SetTaskRunning can only be called by the same AsyncScript.");

            if (to && !TaskRunning)
            {
                TaskRunning = true;
                script.Script.AddTask(Task);
                Script = script;
            }
            else TaskRunning = false;
        }
    }
}

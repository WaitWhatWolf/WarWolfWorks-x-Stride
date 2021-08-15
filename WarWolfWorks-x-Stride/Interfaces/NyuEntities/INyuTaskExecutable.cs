using Stride.Engine;
using System;
using System.Threading.Tasks;

namespace WarWolfWorks_x_Stride.Interfaces.NyuEntities
{
    /// <summary>
    /// Indicates a sub-component which has an executable task.
    /// </summary>
    public interface INyuTaskExecutable
    {
        Func<Task> Task { get; }
        bool TaskRunning { get; set; }
        AsyncScript Script { get; set; }
        public void SetTaskRunning(AsyncScript script, bool to)
        {
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

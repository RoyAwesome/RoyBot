using System;
using System.Collections.Generic;
using System.Text;

namespace RoyBot.InteractionProcess
{
    public class InteractProcessBuilder
    {
        private InteractProcess Process = new InteractProcess();
        private InteractProcess.ProcessStepCallback DefaultCallback;

        public InteractProcessBuilder(string DebugTag)
        {
            Process.DebugTag = DebugTag;
        }

        public InteractProcessBuilder WithDefaultStepCallback(InteractProcess.ProcessStepCallback Callback)
        {
            DefaultCallback = Callback;

            return this;
        }

        public InteractProcessBuilder WithStep(string Prompt, object StepUserObject = null, InteractProcess.ProcessStepCallback Callback = null)
        {
            InteractProcess.ProcessStep Step = new InteractProcess.ProcessStep();

            Step.Description = Prompt;
            if(Callback != null)
            {
                Step.Callback = Callback;
            }
            else if(DefaultCallback != null)
            {
                Step.Callback = DefaultCallback;
            }

            Step.UserObject = StepUserObject;

            Process.ProcessSteps.Enqueue(Step);

            return this;
        }

        public InteractProcessBuilder WithUserObject(object UserObject)
        {
            Process.UserObject = UserObject;
            return this;
        }

        public InteractProcessBuilder WhenSuccessful(InteractProcess.ProcessCallback CompleteCallback)
        {
            Process.OnProcessComplete += CompleteCallback;
            return this;
        }

        public InteractProcessBuilder WhenStarted(InteractProcess.ProcessCallback StartedCallback)
        {
            Process.OnProcessStart += StartedCallback;
            return this;
        }

        public InteractProcessBuilder WhenCancelled(InteractProcess.ProcessCallback CancelCallback)
        {
            Process.OnProcessCancel += CancelCallback;
            return this;
        }

        public InteractProcessBuilder WhenEnded(InteractProcess.ProcessCallback EndedCallback)
        {
            WhenSuccessful(EndedCallback);
            WhenCancelled(EndedCallback);
            return this;
        }

        public InteractProcess Build()
        {
            return Process;
        }

    }
}

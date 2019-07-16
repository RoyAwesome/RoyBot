using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoyBot.InteractionProcess
{
    public class InteractProcess
    {
        public delegate Task ProcessStepCallback(SocketMessage Message, InteractProcess Process, ProcessStep Step);
        public delegate Task ProcessCallback(InteractProcess Process);
       

        public struct ProcessStep
        {
            public string Description;
            public ProcessStepCallback Callback;
            public object UserObject;

            public void Execute(SocketMessage Message, InteractProcess Process)
            {
                if (Callback != null)
                {
                    Callback.Invoke(Message, Process, this);
                }
            }
        }

        public Queue<ProcessStep> ProcessSteps
        {
            get;
            set;
        }

        public IUser InitiatingUser
        {
            get;
            set;
        }

        public ISocketMessageChannel Channel
        {
            get;
            set;
        }

        public DiscordSocketClient Client
        {
            get;
            set;
        }

        public ProcessStep CurrentStep
        {
            get
            {
                return ProcessSteps.Peek();
            }
        }

        public event ProcessCallback OnProcessComplete;
        public event ProcessCallback OnProcessStart;
        public event ProcessCallback OnProcessCancel;
        public event ProcessStepCallback OnProcessStep;

        public object UserObject
        {
            get;
            set;
        }

        public string DebugTag
        {
            get;
            set;
        }

        public InteractProcess()
        {
            ProcessSteps = new Queue<ProcessStep>();
        }

        public ProcessStep GetCurrentProcessStep()
        {
            return ProcessSteps.Peek();
        }

        //return true if there are more steps
        public bool ExecuteStep(SocketMessage Message)
        {
            ProcessStep Step;
            if (ProcessSteps.TryDequeue(out Step))
            {
                Step.Execute(Message, this);

                if (OnProcessStep != null)
                {
                    OnProcessStep.Invoke(Message, this, Step);
                }
            }

            bool MoreSteps = ProcessSteps.Count > 0;

            if(!MoreSteps)           
            {
                if(OnProcessComplete != null)
                {
                    OnProcessComplete.Invoke(this);
                }
               
            }

            return MoreSteps;
        }

        public void Start()
        {
            if(OnProcessStart != null)
            {
                OnProcessStart.Invoke(this);
            }
        }

        public void Cancel()
        {
            if(OnProcessCancel != null)
            {
                OnProcessCancel.Invoke(this);
            }
        }

    }
}

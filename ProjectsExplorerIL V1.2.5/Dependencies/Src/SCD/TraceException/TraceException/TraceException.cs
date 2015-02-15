using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TraceExceptionWrapper
{
    public class TraceException : Exception
    {
        public enum ExeptionType
        {
            Os,
            Application
        }

        public bool ReloadData { get; private set; }
        public String ApplicationName { get; private set; }
        public String ApplicationErrorID { get; private set; }
        public List<String> Parameters { get; private set; }

        public ExeptionType Type;
        public List<StackFrame> Trace { get; private set; } //new StackFrame(1, true)

        public TraceException(String errorID, List<String> parameters, string applicationName)
        {
            Init(ExeptionType.Application, errorID, false, parameters, null, applicationName);
        }

        public TraceException(String errorID, bool reloadData, List<String> parameters, string applicationName)
        {
            Init(ExeptionType.Application, errorID, reloadData, parameters, null, applicationName);
        }

        public TraceException(String errorID, List<String> parameters, StackFrame traceNode, Exception innerExc, string applicationName)
            : base(innerExc.Message, innerExc)
        {
            Init(ExeptionType.Os, errorID, false, parameters, traceNode, applicationName);
        }

        public TraceException(StackFrame traceNode, Exception innerExc, string applicationName)
            : base(innerExc.Message, innerExc)
        {
            Init(ExeptionType.Os, "", false, null, traceNode, applicationName);
        }

        private void Init(ExeptionType type, String applicationErrorID, bool reloadData, List<String> parameters, StackFrame traceNode, string applicationName)
        {
            Type = type;
            ReloadData = reloadData;
            Parameters = parameters;
            Trace = new List<StackFrame>();
            ApplicationErrorID = applicationErrorID;
            ApplicationName = applicationName;

            AddTrace(traceNode);
        }

        public void AddTrace(StackFrame traceNode)
        {
            if (traceNode != null)
                Trace.Insert(0, traceNode);
        }
    }
}

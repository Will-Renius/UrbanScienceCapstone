using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.CognitiveServices.SpeechRecognition;

namespace UrbanScienceCapstone.Models
{
    public class SpeechToText
    {
    }
    public enum SttSTatus { Success, Error };
    public class SpeechToTextEventArgs : EventArgs
    {
        public SttSTatus Status { get; private set; }
        public string Message { get; private set; }
        public List<string> Results { get; private set; }
        public event EventHandler<SpeechToTextEventArgs> OnSttStatusUpdated;
        private DataRecognitionClient _dataRecClient;
        private MicrophoneRecognitionClient m_micClient;
        public SpeechToTextEventArgs(SttSTatus status,
            string message, List<string> results = null)
        {
            Status = status;
            Message = message;
            Results = results;
        
        }
        //gonna need to implement Dispose function

    }
}

using GhumGhamNepal.Core.Services.ServiceResultService;

namespace GhumGham_Nepal.Services
{
    public class ServiceResult : IServiceResult
    {
        public string MessageType { get; set; }

        public List<string> Message { get; set; }

        public bool Status { get; set; }

        public ServiceResult()
        {
            
        }

        public ServiceResult(bool status, List<string> message = null, string messageType = null)
        {
            if (string.IsNullOrEmpty(messageType))
                MessageType = MessageTypeConst.Success;
            else
                MessageType = messageType;
            if (message == null)
                message = new List<string>();
            Message = message;
            Status = status;
        }

        public ServiceResult(bool status, List<string> message)
        {
            Status = status;
            Message = message;
        }

        public static ServiceResult Fail(string message)
        {
            return new ServiceResult(false) { Message = new List<string> { message }, MessageType = MessageTypeConst.Warning };
        }

        public static ServiceResult Fail(string message, string messageType)
        {
            return new ServiceResult(false) { MessageType = messageType, Message = new List<string> { message } };
        }

        public static ServiceResult Fail(List<string> messages)
        {
            return new ServiceResult(false) { Message = messages, MessageType = MessageTypeConst.Warning };
        }

        public static ServiceResult Fail(List<string> messages, string messageType)
        {
            return new ServiceResult(false) { MessageType = messageType, Message = messages };
        }

        public static ServiceResult Success(string message)
        {
            return new ServiceResult(true) { MessageType = MessageTypeConst.Success, Message = new List<string> { message } };
        }
        public static ServiceResult Success(List<string> messages)
        {
            return new ServiceResult(true) { MessageType = MessageTypeConst.Success, Message = messages };
        }
    }

    namespace GhumGham_Nepal.Services
    {
        public class ServiceResult<T> : ServiceResult
        {

            private T ResposeData { get; set; }
            public T Data
            {
                get => this.ResposeData;
                set => this.ResposeData = value;
            }

            public ServiceResult ToResult()
            {
                return new ServiceResult(this.Status, this.Message, this.MessageType);
            }

            public ServiceResult(bool status, List<string> message = null, string messageType = null)
                : base(status, message, messageType)
            {
            }

            public new static ServiceResult<T> Fail(string message)
            {
                return new ServiceResult<T>(false, new List<string> { message }, MessageTypeConst.Warning);
            }
            public new static ServiceResult<T> Fail(string message, string messageType)
            {
                return new ServiceResult<T>(false) { MessageType = messageType, Message = new List<string> { message } };
            }
            public new static ServiceResult<T> Fail(List<string> messages)
            {
                return new ServiceResult<T>(false) { Message = messages, MessageType = MessageTypeConst.Warning };
            }
            public new static ServiceResult<T> Fail(List<string> messages, string messageType)
            {
                return new ServiceResult<T>(false) { MessageType = messageType, Message = messages };
            }

            public new static ServiceResult<T> Success(string message)
            {
                return new ServiceResult<T>(true) { Message = new List<string> { message }, MessageType = MessageTypeConst.Success };
            }

            public static ServiceResult<T> Success(T data)
            {
                return new ServiceResult<T>(true) { Data = data, MessageType = MessageTypeConst.Success };
            }

            public static ServiceResult<T> Success(T data, string message)
            {
                return new ServiceResult<T>(true) { Data = data, Message = new List<string> { message }, MessageType = MessageTypeConst.Success };
            }

            public static ServiceResult<T> Success(T data, List<string> messages)
            {
                return new ServiceResult<T>(true) { Data = data, Message = messages, MessageType = MessageTypeConst.Success };
            }
        }

    }

    public static class MessageTypeConst
    {
        public static string Success => "Success";
        public static string Info => "Info";
        public static string Danger => "Danger";
        public static string Warning => "Warning";
        public static string ValidationFailed => "ValidationFailed";
    }
}

using System.ServiceModel;

namespace FNMES.Service.WebService
{
    [ServiceContract(Namespace = "http://test.namespace")]
    public class WebServiceContract
    {

        [OperationContract]
        public string HelloWorld()
        {
            return "HelloWorld";
        }

        [OperationContract]
        public RetMsg getMsg(string message)
        {
            return new RetMsg { RetCode = 1, Message = message };
        }
    }

    public class RetMsg
    {
        public int RetCode { get; set; }
        public string Message { get; set; }
    }
}
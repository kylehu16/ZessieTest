namespace ZeissTest.Model
{
    [Serializable]
    public class SocketMsg
    {
        public string Topic { get; set; }

        public string Ref { get; set; }

        public Payload Payload { get; set; }

        public string Event { get; set; }
    }
}

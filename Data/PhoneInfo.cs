namespace OutPost13.Data
{
    public class PhoneInfo
    {
        public Provider Providerr { get; set; }
        public string SMSAddress { get; set; }
        public string MMSAddress { get; set; }
        public string DateFirstSeen { get; set; }
        public string DateOfPorting { get; set; }
        public string NoteCodes { get; set; }
        public string NoteDescriptions { get; set; }
        public string TokensUsed { get; set; }

        public class Provider
        {
            public string Name { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string LineType { get; set; }
        }
    }
}

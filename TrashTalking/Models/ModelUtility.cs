using Newtonsoft.Json;

namespace TrashTalking
{
    internal static class ModelUtility
    {
        public static State GetState(string json)
        {
            var simpleState = JsonConvert.DeserializeObject<SimpleState>(json);

            return simpleState.state;
        }
    }
}

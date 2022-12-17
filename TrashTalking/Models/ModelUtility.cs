using Newtonsoft.Json;

namespace TrashTalking
{
    internal static class ModelUtility
    {
        public static ContentType GetContentType(string json)
        {
            var simpleState = JsonConvert.DeserializeObject<EmptyContentType>(json);

            return simpleState.contentType;
        }
    }
}

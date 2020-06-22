using System.IO;
using System.Text.Json;

namespace CollectData
{
    class CancellationUtil
    {
        private const string CancellationStateFile = "CancellationState.txt";

        public static void Save(CancellationState state)
        {
            File.WriteAllText(CancellationStateFile, JsonSerializer.Serialize(state));
        }

        public static CancellationState Restore()
        {
            try
            {
                return JsonSerializer.Deserialize<CancellationState>(File.ReadAllText(CancellationStateFile));
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}

using DocControl.AI;

namespace DocControl
{
    public partial class Form1
    {
        private AiClientFactory? aiClientFactory;

        public Form1(AiClientFactory aiClientFactory) : this()
        {
            this.aiClientFactory = aiClientFactory ?? throw new ArgumentNullException(nameof(aiClientFactory));
        }
    }
}

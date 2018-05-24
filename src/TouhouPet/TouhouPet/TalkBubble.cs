using TouhouPet.Class;

namespace TouhouPet
{
    public partial class TalkBubble : PNGForm
    {
        public TalkBubble() : base(Properties.Resources.TalkBubble)
        {
            InitializeComponent();

            this.Draggable = true;
        }
    }
}

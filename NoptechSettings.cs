using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Noptech
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class NoptechSettings : ISettings
    {
        public int Picture1Id { get; set; }
        public string Text1 { get; set; }
        public string Link1 { get; set; }
        public string AltText1 { get; set; }

        /// <summary>
        /// Gets or sets a widget zone name to place a widget
        /// </summary>
        public string WidgetZone { get; set; }
    }
}
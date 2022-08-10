/* Created by and for usage of FF Studios (2021). */

namespace FFStudio
{
    public class UI_Update_Text_String_Concatenate : UI_Update_Text< SharedStringNotifier, string >
    {
		public string prefix = "";
		public string suffix = "";

		protected override void OnSharedDataChange()
		{
			ui_Text.text = prefix + sharedDataNotifier.SharedValue + suffix;
		}
    }
}
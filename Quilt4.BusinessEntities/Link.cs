namespace Quilt4.BusinessEntities
{
    public class Link
    {
        public string Text { get; private set; }
        public string Action { get; private set; }
        public string Controller { get; private set; }

        public Link(string text, string action, string controller)
        {
            Text = text;
            Action = action;
            Controller = controller;
        }
    }
}
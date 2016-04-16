namespace Bots
{
    public class BotResources
    {
        public virtual string Started() => "Hello!";

        public virtual string Stopped() => "Bye!";

        public virtual string Info() => "No information available. Sorry!";

        public virtual string Help() => "No help available. Sorry!";
    }
}
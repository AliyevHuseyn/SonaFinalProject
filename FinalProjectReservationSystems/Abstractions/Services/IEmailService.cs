namespace FinalProjectReservationSystems.Abstractions.Services
{
    public interface IEmailService
    {
        void Send(string sendTo, string subject, string body, bool isBodyHtml = true);
    }
}

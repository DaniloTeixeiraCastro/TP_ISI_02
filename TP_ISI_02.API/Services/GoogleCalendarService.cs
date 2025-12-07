using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.API.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly string[] Scopes = { CalendarService.Scope.Calendar };
        private readonly string ApplicationName = "TP_ISI_02 Imobiliaria";
        private readonly string CredentialsPath = "credentials.json";

        public async Task<string> CreateEventAsync(Evento evento)
        {
            if (!File.Exists(CredentialsPath))
            {
                return "Google Calendar skipped: 'credentials.json' not found.";
            }

            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream(CredentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                }

                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                var calendarEvent = new Event()
                {
                    Summary = $"Visita (Ref: {evento.Id})",
                    Location = "Local do Imovel",
                    Description = evento.Descricao,
                    Start = new EventDateTime()
                    {
                        DateTime = evento.Data,
                        TimeZone = "Europe/Lisbon",
                    },
                    End = new EventDateTime()
                    {
                        DateTime = evento.Data.AddHours(1),
                        TimeZone = "Europe/Lisbon",
                    },
                };

                // "primary" refers to the primary calendar of the logged-in service account 
                // OR the user email if using Domain-Wide Delegation. 
                // For Service Accounts, we usually share a personal calendar WITH the service account email.
                // But initially, let's try inserting into the Service Account's own primary calendar.
                // The user instructions said to share THEIR calendar with the Service Account.
                // If so, we need the Calendar ID of the user (usually their email).
                // For simplicity MVP, we'll try "primary" (the bot's calendar) first.
                // Ideally, we'd pass a specific CalendarId. 
                // Let's stick to "primary" for now, as the user can check the bot's calendar via API or share it back.
                // Better approach for MVP: The User shares their calendar with the Bot. The Bot inserts into "primary"? 
                // No, if the User shares calendar X with Bot Y, Bot Y must insert into calendar ID X.
                // Since we don't know the user's email hardcoded, we might default to "primary" (Bot's calendar). 
                // Wait, the user instructions: "share your Personal Calendar with that email".
                // This implies the Bot will write to the User's calendar.
                // The request must target the User's Calendar ID (e.g., user@gmail.com).
                string calendarId = "castrodanilo123@gmail.com"; 
                // If "primary", it goes to the Service Account's internal calendar.
                // To go to the user's calendar, we need the user's email here.
                // I will add a comment about this.

                EventsResource.InsertRequest request = service.Events.Insert(calendarEvent, calendarId);
                Event createdEvent = await request.ExecuteAsync();

                return $"Event created: {createdEvent.HtmlLink}";
            }
            catch (Exception ex)
            {
                return $"Error creating event: {ex.Message}";
            }
        }
    }
}

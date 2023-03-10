using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.FPY;
using System.Text;

namespace idrapi.Services
{
    public class MESInformationService : IHostedService
    {
        private readonly IMesRepository _mesRepository;
        private readonly AppDbContext _context;
        private Timer _timer;

        public MESInformationService(IMesRepository mesRepository)
        {
            _mesRepository = mesRepository;
            _context = new AppDbContext();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            TimeSpan interval = TimeSpan.FromHours(24);
            //calculate time to run the first time & delay to set the timer
            //DateTime.Today gives time of midnight 00.00

            // Para produccion
            var nextRunTime = DateTime.Today.AddDays(1).AddHours(1);

            // PAra pruebas
            //var nextRunTime = DateTime.Now.AddSeconds(15);

            var curTime = DateTime.Now;
            var firstInterval = nextRunTime.Subtract(curTime);

            Action action = () =>
            {
                var t1 = Task.Delay(firstInterval);
                t1.Wait();
                //remove inactive accounts at expected time

                //now schedule it to be called every 24 hours for future
                // timer repeates call to RemoveScheduledAccounts every 24 hours.
                _timer = new Timer(
                    GetInformationFromMES,
                    null,
                    TimeSpan.Zero,
                    interval
                );            
            };

            // no need to await this call here because this task is scheduled to run much much later.
            Task.Run(action);
            return Task.CompletedTask;
        }

        /// Call the Stop async method if required from within the app.

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void GetInformationFromMES(object state)
        {
            var yesterday = DateTime.Today.AddDays(-1);
            var today = DateTime.Today;
            Console.WriteLine("AUTOMATIC CALL TO MES...");
            Console.WriteLine($"DAY: {DateTime.Now}");
            Console.WriteLine($"GETTING INFORMATION FROM {yesterday} to {today}");
            await GetProducedFromMes( yesterday, today);
            await GetFailsFromMes(yesterday, today);
            Console.WriteLine("INFORMATION SAVED.");
            Console.WriteLine("========================================" + Environment.NewLine);
        }

        private async Task GetProducedFromMes(DateTime yesterday, DateTime today)
        {

            List<ProducedAndFilteredFPY> FPYData = _mesRepository.GetProducedAndFiltereds(yesterday, today);

            await _context.ProducedAndFilteredFPYs.AddRangeAsync(FPYData);

            await _context.SaveChangesAsync();
        }

        private async Task GetFailsFromMes(DateTime yesterday, DateTime today)
        {

            List<FailureFPY> FPYFail = _mesRepository.GetRAW_Fails(yesterday, today);

            await _context.FailuresFPY.AddRangeAsync(FPYFail);
            await _context.SaveChangesAsync();
        }

    }
}

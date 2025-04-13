using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace SmartGreenhouse._2._0_Core
{
    public class Sensor
    {
        private readonly List<Measurement> _measurements = new List<Measurement>();

        public int SensorId { get; private set; }

        /// <summary>
        /// Type sensor. Verplicht, ingesteld via constructor.
        /// </summary>
        [Required(ErrorMessage = "Sensor type is verplicht.")]
        public string Type { get; private set; } // Private set

        public string? Name { get; set; }

        // --- Relatie naar Greenhouse ---
        /// <summary>
        /// Foreign Key naar Greenhouse. Wordt intern beheerd.
        /// </summary>
        public int GreenhouseId { get; private set; }

        public Greenhouse Greenhouse { get; private set; }

        // --- Relatie naar Measurements ---
        /// <summary>
        /// Publieke, alleen-lezen weergave van de metingen.
        /// </summary>
        public IReadOnlyCollection<Measurement> Measurements => _measurements.AsReadOnly();

        /// <summary>
        /// Parameterloze constructor nodig voor EF Core en model binding.
        /// </summary>
        public Sensor()
        {
            Type = string.Empty;
        }

        public Sensor(string type, Greenhouse greenhouse, string? name = null) : this() // this calls the parameterless constructor
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type), "Sensor type mag niet leeg zijn.");
            if (greenhouse == null)
                throw new ArgumentNullException(nameof(greenhouse), "Sensor moet aan een kas gekoppeld zijn.");

            Type = type;
            Name = name;
            // Koppelen via de Greenhouse methode zorgt voor consistentie in beide objecten
            greenhouse.AddSensor(this);
            // SetGreenhouseInternal is nu impliciet aangeroepen door AddSensor
        }

        /// <summary>
        /// Voegt een meting toe aan deze sensor en beheert de relatie.
        /// </summary>
        public void AddMeasurement(Measurement measurement)
        {
            if (measurement == null) throw new ArgumentNullException(nameof(measurement));
            if (!_measurements.Any(m => (m.MeasurementId != 0 && m.MeasurementId == measurement.MeasurementId) || object.ReferenceEquals(m, measurement)))
            // voorkomt dat dezelfde meting meerdere keren wordt toegevoegd, met behulp van gemini 1.5 pro ai gemaakt
            {
                // Zet de relatie vanuit measurement correct (via interne methode)
                if (measurement.Sensor != this)
                {
                    measurement.SetSensorInternal(this);
                }
                _measurements.Add(measurement);
            }
        }

        /// <summary>
        /// Interne methode (binnen dezelfde assembly) om de Greenhouse relatie te zetten.
        /// Wordt aangeroepen vanuit Greenhouse.AddSensor/RemoveSensor.
        /// </summary>
        internal void SetGreenhouseInternal(Greenhouse? greenhouse)
        {
            // Als de nieuwe kas verschilt van de huidige
            if (!object.ReferenceEquals(this.Greenhouse, greenhouse))
            {
                // Verwijder van de eventuele oude kas (via interne methode)
                this.Greenhouse?.RemoveSensorInternal(this);
            }
            // Zet de nieuwe kas en FK
            this.Greenhouse = greenhouse;
            this.GreenhouseId = greenhouse?.GreenhouseId ?? 0;
        }
    }
}

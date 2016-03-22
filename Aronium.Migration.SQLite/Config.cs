using System.IO;
using System.Xml.Serialization;

namespace Aronium.Migration
{
    public class Config
    {
        #region - Fields -

        private static Config _instance;

        #endregion

        #region - Constructor -

        private Config() { }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets database.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets Config instance.
        /// </summary>
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Deserialize();
                }

                return _instance;
            }
        } 

        #endregion

        #region - Public methods -

        /// <summary>
        /// Save instance to a file.
        /// </summary>
        public static void Save()
        {
            using (TextWriter writer = new StreamWriter(GetConfigFilename(), false))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                serializer.Serialize(writer, Config.Instance, ns);

                writer.Close();
            }

        }

        #endregion

        #region - Private methods -

        /// <summary>
        /// Gets config properties file path.
        /// </summary>
        /// <returns>File path.</returns>
        private static string GetConfigFilename()
        {
            var directory = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;
            return Path.Combine(directory, "migration.properties");
        }

        /// <summary>
        /// Deserializes or initializes new instance from properties file, if any.
        /// </summary>
        /// <returns>Instance of Config.</returns>
        private static Config Deserialize()
        {
            var filename = GetConfigFilename();

            if (File.Exists(filename))
            {
                using (TextReader reader = new StreamReader(filename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Config));

                    object o = serializer.Deserialize(reader);
                    reader.Close();

                    return (Config)o;
                }
            }
            else
            {
                return new Config();
            }
        } 

        #endregion
    }
}

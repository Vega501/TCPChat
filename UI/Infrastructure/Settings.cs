﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using Keys = System.Windows.Forms.Keys;

namespace UI.Infrastructure
{
  public class Settings
  {
    private static Settings current;
    private static readonly object syncObj = new object();
    private const string FileName = "Settings.xml";

    public static Settings Current
    {
      get
      {
        if (current != null)
          return current;

        lock (syncObj)
        {
          if (current == null)
          {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
            if (!File.Exists(fileName))
              return current = GetDefault();

            var serializer = new XmlSerializer(typeof(Settings));
            using (var stream = File.Open(fileName, FileMode.Open))
              current = (Settings)serializer.Deserialize(stream);
          }
        }

        return current;
      }
    }

    private static Settings GetDefault()
    {
      return new Settings
      {
        Locale = "en-US",

        Nick = "User",
        NickColor = Color.FromArgb(170, 50, 50),
        RandomColor = true,

        FormSize = new Size(380, 470),
        Alerts = true,
        Address = "127.0.0.1",
        Port = 10021,
        ServicePort = 10022,
        StateOfIPv6Protocol = false,

        RecorderKey = Keys.E,
        Frequency = 44100,
        Bits = 16
      };
    }

    public static void SaveSettings()
    {
      if (current == null)
        return;

      lock (syncObj)
      {
        var serializer = new XmlSerializer(typeof(Settings));
        using (var stream = File.Create(AppDomain.CurrentDomain.BaseDirectory + FileName))
          serializer.Serialize(stream, current);
      }
    }

    #region properties
    public string Locale { get; set; }

    public string Nick { get; set; }
    public SavedColor NickColor { get; set; }
    public bool RandomColor { get; set; }

    public Size FormSize { get; set; }
    public bool Alerts { get; set; }

    public string Address { get; set; }
    public int Port { get; set; }
    public int ServicePort { get; set; }
    public bool StateOfIPv6Protocol { get; set; }

    public Keys RecorderKey { get; set; }
    public string OutputAudioDevice { get; set; }
    public string InputAudioDevice { get; set; }
    public int Frequency { get; set; }
    public int Bits { get; set; }

    public List<PluginSetting> Plugins { get; set; }
    #endregion

    public class SavedColor
    {
      private SavedColor() { }

      public byte R { get; set; }
      public byte G { get; set; }
      public byte B { get; set; }

      public static implicit operator Color(SavedColor color)
      {
        return Color.FromArgb(color.R, color.G, color.B);
      }

      public static implicit operator SavedColor(Color color)
      {
        return new SavedColor { R = color.R, G = color.G, B = color.B };
      }
    }
  }
}

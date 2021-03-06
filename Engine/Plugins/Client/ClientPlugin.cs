﻿namespace Engine.Plugins.Client
{
  /// <summary>
  /// Представляет базовый класс для реализации клиентского плагина.
  /// </summary>
  public abstract class ClientPlugin :
    Plugin<ClientModelWrapper, ClientPluginCommand>
  {
    public abstract string MenuCaption { get; }
    public abstract void InvokeMenuHandler();
  }
}

﻿using Engine.Model.Client;
using Engine.Model.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.Infrastructure;

namespace UI.ViewModel
{
  public class PostedFilesViewModel : BaseViewModel
  {
    public ObservableCollection<PostedFileRoomViewModel> Rooms { get; private set; }

    public PostedFilesViewModel(BaseViewModel parent)
      : base(parent, false)
    {
      Rooms = new ObservableCollection<PostedFileRoomViewModel>();

      using (var client = ClientModel.Get())
      {
        var items = new Dictionary<string, PostedFileRoomViewModel>();

        foreach (var file in client.PostedFiles)
        {
          PostedFileRoomViewModel item;
          if (!items.TryGetValue(file.RoomName, out item))
          {
            item = new PostedFileRoomViewModel(client, file.RoomName, this);
            items.Add(file.RoomName, item);
            Rooms.Add(item);
          }

          item.PostedFiles.Add(new PostedFileViewModel(client, file, item));
        }
      }
    }

    public void RemoveRoom(PostedFileRoomViewModel item)
    {
      Rooms.Remove(item);
      item.Dispose();
    }
  }

  public class PostedFileRoomViewModel : BaseViewModel
  {
    private PostedFilesViewModel parent;

    public string RoomName { get; private set; }
    public ObservableCollection<PostedFileViewModel> PostedFiles { get; private set; }

    public PostedFileRoomViewModel(ClientGuard client, string roomName, PostedFilesViewModel parent)
      : base(parent, false)
    {
      this.parent = parent;

      RoomName = roomName;
      PostedFiles = new ObservableCollection<PostedFileViewModel>();
    }

    public void RemoveFile(PostedFileViewModel item)
    {
      PostedFiles.Remove(item);
      item.Dispose();

      if (PostedFiles.Count == 0)
        parent.RemoveRoom(this);
    }
  }

  public class PostedFileViewModel : BaseViewModel
  {
    private PostedFileRoomViewModel parent;

    public FileId FileId { get; private set; }
    public string FileName { get; private set; }

    public ICommand RemoveCommand { get; private set; }

    public PostedFileViewModel(ClientGuard client, PostedFile postedFile, PostedFileRoomViewModel parent)
      : base(parent, false)
    {
      this.parent = parent;

      FileId = postedFile.File.Id;
      FileName = postedFile.File.Name;
  
      RemoveCommand = new Command(Remove, _ => ClientModel.Api != null);
    }

    private void Remove(object o)
    {
      parent.RemoveFile(this);

      ClientModel.Api.RemoveFileFromRoom(parent.RoomName, FileId);
    }
  }
}

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Utility
{
    public static class LoadAudioHelper
    {
        public const string MP3 = ".mp3";
        public const string OGG = ".ogg";
        public const string WAV = ".wav";
        
        public static AudioType GetTypeFromPath(string path)
        {
            string extension = Path.GetExtension(path);
            switch (extension)
            {
                case MP3:
                    return AudioType.MPEG;
                case OGG:
                    return AudioType.OGGVORBIS;
                case WAV:
                    return AudioType.WAV;
                default:
                    return AudioType.UNKNOWN;
            }
        }

        public static IEnumerator Load(string filename, Action<AudioClip> onComplete, Action<string> onFailure)
        {
            string macLocation = "file://" + filename; // TODO, this is bad
            string location = macLocation;

            AudioType type = GetTypeFromPath(filename);
            if (type == AudioType.UNKNOWN)
            {
                string error = $"Unsupported file type/extension {Path.GetExtension(filename)}";
                onFailure.Invoke(error);
            }
            
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(location, GetTypeFromPath(filename)))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    onFailure.Invoke(www.error);
                }
                else
                {
                    onComplete.Invoke(DownloadHandlerAudioClip.GetContent(www));
                }
            }
        }

        public static AudioClip MakeCopy(this AudioClip toCopy)
        {
            return GameObject.Instantiate(toCopy);
        }
    }
}
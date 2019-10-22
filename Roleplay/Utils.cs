﻿using UnityEngine;
using System.IO;
using NovaLife.Server.Gamemode;

public static class Utils
{
    public static Roleplay gamemode;

    public static void AccountsInit()
    {
        // Si le répertoire des comptes n'existe pas on le créé
        string path_accounts = Application.dataPath + "/../Servers/" + gamemode.serverName + "/Accounts/";
        if (!Directory.Exists(path_accounts))
        {
            Directory.CreateDirectory(path_accounts);
        }
    }

    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    // Fonction pour tester si on peut connecter une compte
    public static bool LoginAccount(string steamId, uint playerId, string password)
    {
        string path_accounts = Application.dataPath + "/../Servers/" + gamemode.serverName + "/Accounts/";

        if (File.Exists(path_accounts + steamId + ".json"))
        {
            string accountStr = File.ReadAllText(path_accounts + steamId + ".json");
            Account loginAccount = JsonUtility.FromJson<Account>(accountStr);

            if (loginAccount.password == Utils.CreateMD5(password))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }

    // Fonction pour inscrire le compte dans le dossier Accounts du serveur
    public static void RegisterAccount(Account account, uint playerId)
    {
        string path_accounts = Application.dataPath + "/../Servers/" + gamemode.serverName + "/Accounts/";

        if (!File.Exists(path_accounts + account.steamId + ".json"))
        {
            File.WriteAllText(path_accounts + account.steamId + ".json", JsonUtility.ToJson(account));
            gamemode.OnRegisterAccount(playerId);

            gamemode.players[playerId].Add("isFirstConnect", "1");
        }
    }

    public static void CreateOpinion(uint playerId, string opinion)
    {
        // Si le répertoire des avis n'existe pas on le créé
        string path_opinions = Application.dataPath + "/../Servers/" + gamemode.serverName + "/Opinions/";
        if (!Directory.Exists(path_opinions))
        {
            Directory.CreateDirectory(path_opinions);
        }

        if(string.IsNullOrEmpty(opinion))
        {
            gamemode.SendClientMessage(playerId, "#ffffff", "Votre avis ne doit pas être vide !");
        }else
        {
            string steamId = gamemode.players[playerId]["steamid"];

            if (!File.Exists(path_opinions + steamId + ".json"))
            {
                Opinion _opinion = new Opinion();
                _opinion.steamId = steamId;
                _opinion.opinion = opinion;

                File.WriteAllText(path_opinions + steamId + ".json", JsonUtility.ToJson(_opinion));
            }
            else
            {
                gamemode.SendClientMessage(playerId, "#ffffff", "Vous avez déjà soumis votre avis, merci ! Nous vous redemanderons votre avis à la prochaine mise à jour de notre mode de jeu !");
            }
        }
        
    }

    public static void JobsInit()
    {
        // Si le répertoire des métiers n'existe pas on le créé
        string path_jobs = Application.dataPath + "/../Servers/" + gamemode.serverName + "/Jobs/";
        if (!Directory.Exists(path_jobs))
        {
            Directory.CreateDirectory(path_jobs);
        }

        // On récupère tous les dossiers de chaque métier pour effectuer une itération
        string[] jobs_directories = Directory.GetDirectories(path_jobs);

        // On charge tous les métiers à partir du répertoire que l'on a créé auparavant
        for (int i = 0; i < jobs_directories.Length; i++)
        {
            if (File.Exists(jobs_directories[i] + "config.json"))
            {
                string jobStr = File.ReadAllText(jobs_directories[i] + "config.json");
                Job job = JsonUtility.FromJson<Job>(jobStr);

                gamemode.jobs.Add(job.jobId, job);
            }
        }
    }
}
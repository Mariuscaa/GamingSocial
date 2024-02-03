namespace HIOF.GamingSocial.GUI.Model.GameInformation;

using System;
using System.Collections.Generic;


    public class ProfileWithGames
    {
        public Guid ProfileGuid { get; set; }
        public List<GameRatings> GamesCollection { get; set; }

        public ProfileWithGames()
        {

        }
    }


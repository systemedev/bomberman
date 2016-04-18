using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bomberman.Models
{
    //view model for GameRequestController Index to merge what i've send and what i received
    public class GameRequestReceivedSentViewModel
    {
        public IList<Games_requests> input {get;set;}
          
    public IList<Games_requests> output {get;set;}
        
    }
}
using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insyston.Operations.WPF.ViewModel.Events
{    
    public class SearchColumnFiltersLoaded : CompositePresentationEvent<Dictionary<string, List<string>>>
    {
    }
}

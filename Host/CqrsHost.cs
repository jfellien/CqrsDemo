﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastruktur.Common;
using Infrastruktur.EventSourcing;
using Infrastruktur.Messaging;
using Readmodels;

namespace Host
{
    
    public partial class CqrsHost : Port
    {

        private readonly EventStore _eventStore;
        private readonly KundeReadmodel _kunden;
        private readonly ProduktReadmodel _produkte;
        private readonly AuftragReadmodel _auftraege;
        private readonly MetaReadmodel _meta;

        public CqrsHost(EventStore eventStore)
        {
            if (eventStore == null) throw new ArgumentNullException("eventStore");
            _eventStore = eventStore;

            _kunden = new KundeReadmodel(_eventStore.Retrieve);
            _auftraege = new AuftragReadmodel(_eventStore.Retrieve);
            _produkte = new ProduktReadmodel(_eventStore.Retrieve);
            _meta = new MetaReadmodel(_eventStore.Subscribe);

        }

        public void Handle(Command command)
        {
            Handle(command, (dynamic)command.Aktion);
        }

        public Resource<T> Retrieve<T>(Query query) where T : class
        {
            var resource = Handle(query, (dynamic)query.Abfrage);
            if (resource == null)
            {
                throw new InvalidOperationException(string.Format("Query Handler liefert kein Ergebnis! {0}",
                                                                  query.Abfrage.GetType().Name));
            }

            if (resource is Resource<T>)
            {
                return (Resource<T>) resource;
            }

            var t = resource as T;
            if (t == null)
            {
                throw new InvalidOperationException(
                    string.Format("Query Handler liefert unerwartetes Ergebnis! {0} {1} statt {2}",
                                  query.Abfrage.GetType().Name, resource.GetType().Name, typeof (T).Name));
            }

            return new Resource<T> {Body = t};
        }

        private void Handle(Command command, object aktion)
        {
            throw new NotImplementedException(string.Format("Kein Command Handler für {0} definiert", aktion.GetType().Name));
        }

        private object Handle(Query query, object abfrage)
        {
            throw new NotImplementedException(string.Format("Kein Query Handler für {0} definiert", abfrage.GetType().Name));
        }


    }


}

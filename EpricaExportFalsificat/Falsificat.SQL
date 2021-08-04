select 
hex(es.name) as good_name, 
hex(es_producer.producer_name) as producer_name ,
'' as country,
hex(falsificat.Seria) as Seria,
hex(falsificat_reason.name) as reason,
hex(cast( falsificat.DocDate as text)) as docdate,
hex (falsificat.DocNum) as DocNum,
hex(lab.name) as lab_name
from es , es_producer, falsificat, lab , falsificat_2_reason, falsificat_reason
where es.kod_producer = es_producer.kod_producer
and es.guid_es = falsificat.GuidEs
AND falsificat.IdLab = lab.id
and falsificat.IdFalsificat = falsificat_2_reason.id_falsificat
and falsificat_2_reason.id_reason  = falsificat_reason.id
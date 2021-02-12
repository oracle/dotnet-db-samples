CREATE TABLE j_purchaseorder
  (id          VARCHAR2 (32) NOT NULL PRIMARY KEY,
   date_loaded TIMESTAMP (6) WITH TIME ZONE,
   po_document JSON);

INSERT INTO j_purchaseorder
  VALUES (SYS_GUID(),
          to_date('30-DEC-2014'),
          '{"PONumber"             : 1600,
            "Reference"            : "ABULL-20140421",
	          "Requestor"            : "Alexis Bull",
            "User"                 : "ABULL",
            "CostCenter"           : "A50",
            "ShippingInstructions" : "Mail package",
            "Special Instructions" : null,
            "AllowPartialShipment" : true,
            "LineItems"            : "Hardware"}');

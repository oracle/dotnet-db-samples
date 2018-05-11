
CREATE OR REPLACE PACKAGE "MYPACK" AS
TYPE AssocArrayVarchar2_t is table of VARCHAR(20) index by BINARY_INTEGER;
          PROCEDURE MYSP(
            Param1 IN     AssocArrayVarchar2_t,
            Param3    OUT AssocArrayVarchar2_t);
          END "MYPACK";
/

CREATE OR REPLACE PACKAGE BODY "MYPACK" as 
  PROCEDURE MYSP(
  Param1 IN     AssocArrayVarchar2_t,
  Param3    OUT AssocArrayVarchar2_t)
  IS
  BEGIN
    Param3(1) := Param1(3);
    Param3(2) := Param1(1);
    Param3(3) := Param1(2);
  END MYSP;
END "MYPACK";
/

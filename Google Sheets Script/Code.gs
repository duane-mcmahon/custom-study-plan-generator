function onOpen(key)
{ 
    
   setCellColors("A1:A6", "black", "#b7b7b7", key);
   
   setCellColors("B1:B6", "black", "#f3f3f3", key);
   
   setCellColors("C1:C6", "black", "#f3f3f3", key);
   
   setCellColors("D1:D6", "black", "#f3f3f3", key);
   
   setCellColors("E1:E6", "black", "#f3f3f3", key);
   
   setCellFont("A9", "Verdana", 14, key);
   
   setCellWidths(key);

};




function setCellColors(range, foregrd, backgrd, id) {
   var sheet = SpreadsheetApp.openById(id).getSheets()[0];     // Select the first sheet.
   var cell = sheet.getRange(range);               // Use supplied arguments
   cell.setFontColor(foregrd);                     // to set font and
   cell.setBackground(backgrd);                    // background colours.
   
   
 }
 
 function setCellFont(range, family, size, id) {
   var sheet = SpreadsheetApp.openById(id).getSheets()[0];     // Select the first sheet.
   var cell = sheet.getRange(range);               // Use supplied arguments
   cell.setFontFamily(family);                  // to set font
   cell.setFontSize(size);          
 }
 
 function setCellWidths(id){
 
    var sheet = SpreadsheetApp.openById(id).getSheets()[0];
     sheet.autoResizeColumn(1);
     sheet.autoResizeColumn(2);
     sheet.autoResizeColumn(3);
     sheet.autoResizeColumn(4);
     sheet.autoResizeColumn(5);
     sheet.autoResizeColumn(6);
     
 
 }
 

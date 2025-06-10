Attribute VB_Name = "NewMacros"
Sub UpdatePageNumbersInTable()
    Dim tbl As Table
    Dim cell As cell
    Dim pageNum As Integer
    Dim totalPages As Integer
    Dim i As Long
    Dim text As String
    Dim word As Variant
    Dim deleteTable As Boolean
    
    ' ��� 1: �������� ������ � ��������� "#"
    For i = ActiveDocument.Tables.Count To 1 Step -1
        Set tbl = ActiveDocument.Tables(i)
        deleteTable = False
        
        ' ��������� ��� ������ �������
        For Each cell In tbl.Range.Cells
            text = cell.Range.text
            ' ��������� ����� �� �����
            For Each word In Split(text, " ")
                ' ��������� �����, ������������ �� #
                If Left(Trim(word), 1) = "#" And Len(Trim(word)) > 1 Then
                    deleteTable = True
                    Exit For
                End If
            Next word
            If deleteTable Then Exit For
        Next cell
        
        ' ������� �������, ���� ������ ������ #
        If deleteTable Then
            tbl.Delete
        End If
    Next i
    
    ' ��� 2: ���������� ������� �������
    totalPages = ActiveDocument.Content.Information(wdNumberOfPagesInDocument)
    
    For Each tbl In ActiveDocument.Tables
        For Each cell In tbl.Range.Cells
            ' ��������� [PAGE]
            If InStr(cell.Range.text, "[PAGE]") > 0 Then
                pageNum = cell.Range.Information(wdActiveEndPageNumber)
                cell.Range.text = Replace(cell.Range.text, "[PAGE]", CStr(pageNum))
            End If
            
            ' ��������� [MAXPAGE]
            If InStr(cell.Range.text, "[MAXPAGE]") > 0 Then
                cell.Range.text = Replace(cell.Range.text, "[MAXPAGE]", CStr(totalPages))
            End If
        Next cell
    Next tbl
    
    ' �������������� ���������� ���������
    ActiveDocument.Fields.Update
End Sub

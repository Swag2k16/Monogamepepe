﻿Public Class World

    Private frontiers As List(Of Cell)
    Public Const rows As Integer = 11
    Public Const columns As Integer = 11

    Private Grid(rows - 1, columns - 1) As Cell

    Public Sub New()
        'Initialize cells
        For row = 0 To rows - 1
            For column = 0 To rows - 1
                Grid(row, column) = New Cell(row, column)
            Next
        Next
        Dim r As Integer = (rows - 1) / 2
        Dim c As Integer = (columns - 1) / 2

        Grid(r, c).type = Cell.types.FLOOR

        frontiers = getFrontiers(r, c)
        'Create maze
        PrimsMaze()
    End Sub

    Public Function GetCell(row As Integer, column As Integer) As Cell
        Return Grid(row, column)
    End Function

    Private Function randomChar(ByRef charlist As List(Of Char))
        Dim randchar = charlist(Game.rnd.Next(0, charlist.Count))
        Return randchar
    End Function

    Private Function getNearCells(ByVal row As Integer, ByVal column As Integer) As List(Of Cell)
        Dim NearCells As List(Of Cell) = New List(Of Cell)

        'Up
        If row - 2 >= 0 Then
            NearCells.Add(Grid(row - 2, column))
        End If

        'Down
        If row + 2 <= rows - 1 Then
            NearCells.Add(Grid(row + 2, column))
        End If

        'Left
        If column + 2 <= columns - 1 Then
            NearCells.Add(Grid(row, column + 2))
        End If

        'Right
        If column - 2 >= 0 Then
            NearCells.Add(Grid(row, column - 2))
        End If

        Return NearCells
    End Function

    Private Function getFrontiers(ByVal row As Integer, ByVal column As Integer) As List(Of Cell)
        Dim frontiers As List(Of Cell) = getNearCells(row, column)
        'Remove all walls
        frontiers.RemoveAll(Function(c) Not c.type = Cell.types.WALL)
        Return frontiers
    End Function

    Private Function getNeigbors(ByVal row As Integer, ByVal column As Integer) As List(Of Cell)
        Dim neigbors As List(Of Cell) = getNearCells(row, column)
        'Remove all walls
        neigbors.RemoveAll(Function(c) c.type = Cell.types.WALL)
        Return neigbors
    End Function

    Public Sub PrimsMaze()
        While frontiers.Count > 0
            'Pick a random frountier and neigbor
            Dim frontier As Cell = frontiers(Game.rnd.Next(0, frontiers.Count - 1))
            Dim neigbors As List(Of Cell) = getNeigbors(frontier.Row, frontier.Column)
            Dim neigbor As Cell = neigbors(Game.rnd.Next(0, neigbors.Count - 1))

            'Remove wall between frountier and passage
            'frontier.print()
            'neigbor.print()
            If frontier.Row = neigbor.Row Then
                If frontier.Column > neigbor.Column Then
                    Grid(frontier.Row, frontier.Column - 1).type = Cell.types.FLOOR
                ElseIf frontier.Column < neigbor.Column Then
                    Grid(frontier.Row, frontier.Column + 1).type = Cell.types.FLOOR
                End If
            ElseIf frontier.Column = neigbor.Column Then
                If frontier.Row > neigbor.Row Then
                    Grid(frontier.Row - 1, frontier.Column).type = Cell.types.FLOOR
                ElseIf frontier.Row < neigbor.Row Then
                    Grid(frontier.Row + 1, frontier.Column).type = Cell.types.FLOOR
                End If
            End If

            frontier.type = Cell.types.FLOOR
            frontiers.AddRange(getFrontiers(frontier.Row, frontier.Column))
            frontiers.Remove(frontier)
            frontiers.RemoveAll(Function(j) Not j.type = Cell.types.WALL)
        End While
        Grid(1, 1).type = Cell.types.START
        Grid(rows - 2, columns - 2).type = Cell.types.ENDPOINT
    End Sub

    Public Sub RegenerateMaze()
        'Initialize cells
        For row = 0 To rows - 1
            For column = 0 To rows - 1
                Grid(row, column) = New Cell(row, column)
            Next
        Next
        Dim r As Integer = (rows - 1) / 2
        Dim c As Integer = (columns - 1) / 2

        Grid(r, c).type = Cell.types.FLOOR

        frontiers = getFrontiers(r, c)
        'Create maze
        PrimsMaze()
    End Sub
End Class
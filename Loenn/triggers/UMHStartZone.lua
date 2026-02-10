local UMHStartZone = {}

UMHStartZone.name = "UMH/UMHStartZone"
UMHStartZone.placements = {
    {
        name = "default",
        data = {
            room = "arena"
        }
    }
}
UMHStartZone.fieldInformation = {
    room = {
        fieldType = "string",
        default = "arena"
    }
}

return UMHStartZone
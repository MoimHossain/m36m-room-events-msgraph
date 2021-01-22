
Install-Module -Name ExchangeOnlineManagement

Connect-ExchangeOnline -UserPrincipalName moim@cloudoven.onmicrosoft.com

# Creating Room List
New-DistributionGroup -Name "Conference Rooms" -PrimarySmtpAddress "ConfRooms@cloudoven.onmicrosoft.com" -RoomList

# Check Room list
Get-Mailbox -RecipientTypeDetails RoomMailBox

# Add room to room-list
Add-DistributionGroupMember -Identity "Conference Rooms" -Member "OvalOffice"
Add-DistributionGroupMember -Identity "Conference Rooms" -Member "Scrum"
Add-DistributionGroupMember -Identity "Conference Rooms" -Member "Atrium"

# Verify room list
 Get-DistributionGroupMember -Identity "Conference Rooms"

 # Get mail boxes
 Get-Mailbox -ResultSize unlimited -Filter "RecipientTypeDetails -eq 'RoomMailbox'" | Get-CalendarProcessing | Format-List Identity,ScheduleOnlyDuringWorkHours,MaximumDurationInMinutes


 # Set equipments
 Set-Place -Identity "Scrum" -IsWheelChairAccessible $true -AudioDeviceName PolyCom -VideoDeviceName "InFocus WXGA Projector"
 Set-Place -Identity "OvalOffice" -IsWheelChairAccessible $true -AudioDeviceName PolyCom -VideoDeviceName "InFocus WXGA Projector"
 Set-Place -Identity "Atrium" -IsWheelChairAccessible $true -AudioDeviceName PolyCom -VideoDeviceName "InFocus WXGA Projector"

 # Set location
Set-Place -Identity "OvalOffice" -Building "HQ"
Set-Place -Identity "Scrum" -Building "HQ"                                                                                                                                                                                [08:24]
Set-Place -Identity "Atrium" -Building "HQ"

Set-Place -Identity "OvalOffice" -City "Zoetermeer"
Set-Place -Identity "Scrum" -City "Zoetermeer"                                                                                                                                                                                [08:24]
Set-Place -Identity "Atrium" -City "Zoetermeer"



# Scoping Confidential clients to certan mailboxes
# https://docs.microsoft.com/en-us/graph/auth-limit-mailbox-access

New-ApplicationAccessPolicy -AppId 65f46078-6eef-41f5-b40d-eb7d11bb2cbf -PolicyScopeGroupId meeting-rooms@cloudoven.onmicrosoft.com -AccessRight RestrictAccess -Description "Restrict this app to members of distribution group EvenUsers."  

# Test it
Test-ApplicationAccessPolicy -Identity meeting-rooms@cloudoven.onmicrosoft.com -AppId 65f46078-6eef-41f5-b40d-eb7d11bb2cbf
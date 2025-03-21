There's 2 controllers to the project:
 - Chats controller
 - Shift controller

To start, call the create shift endpoint first defining the number of members in the team as well as if an overflow team is available. This will create a shift as well as the chat queues to be used by the shift.
When the shift is done, call the end shift endpoint to end the shift and delete the chat queues.

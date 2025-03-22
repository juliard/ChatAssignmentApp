There's 2 controllers to the project:
 - Chats controller
 - Shift controller

To start, call the create shift endpoint first defining the number of members in the team as well as if an overflow team is available. This will create a shift as well as the chat queues to be used by the shift.
When the shift is done, call the end shift endpoint to end the shift and delete the chat queues when no active chats are assigned to any agent including overflow agents.
If chats are still active, the shift will not be able to process any new chats except the ones already queued in the main chat queue and the overflow chat queue.
If this happens, call the end shift endpoint again to close the shift completely.

To queue a chat, create a chat. Make sure there is an active shift.
Chats will be assigned to the agents on a round robin basis following the rule junior, mid, senior, then leads.
A chat will be inactive when no response is received after 3 seconds.
Once a chat is inactive, it will be removed from the agent active chat list so that another chat can be processed by the agent.

Queue size is defined by the number of active agents available * 1.5. Agent-supported concurrent chats are as follows:
 - Junior - 4
 - Mid - 6
 - Senior - 8
 - Lead - 5

If there's one of each, the max queue size is 34.5 (rounded down to 34).
If there's 2 juniors, 3 mid, 0 senior, and 2 leads, the max queue size is 54.
Overflow queue size is also dependent on the number of active agents and each overflow agent can only handle 4 concurrent chats like a junior agent.

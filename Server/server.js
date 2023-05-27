import express from 'express'
import cors from 'cors'
import { config } from "dotenv"
config()

import { Configuration, OpenAIApi } from "openai"

const openAi = new OpenAIApi(
  new Configuration({
    apiKey: process.env.API_KEY,
  })
)

const app = express();
app.use(cors());
app.use(express.json());
const port = 5000;

app.get('/', (req, res)=> {
  res.send('SpotlightGPT Server');
})

app.listen(port, () => console.log("i hope this works"))

app.post('/', async (req, res) => {

  const requestData = req.body;

  const response = await openAi.createChatCompletion({
    model: "gpt-3.5-turbo",
    messages: [
      { role: "system", content: "You're an AI Language Model that powers a windows app called spotlight GPT. SpotlightGPT is similar to MacOs spotlight but it's for windows and has the capabilities to do anything that the user can do. You're like a windows personal assistant, helping the user do anything on their windows computer. Your outputs range from normal conversations to providing specific parameters for specific functions. When providing outputs you must first figure out what the user wants, find out find out which of the functions the request goes with, and lastly return the proper parameters to that function. For example if you're given the prompt \" Volume 34 \" you should know the user wants their volume set to 34 and then send the appropriate parameters. Your outputs must never be full sentences unless the you think they're asking some sort of question. If your instructions are vauge then you must assume what the user wants."},
      { role: "system", content: "Task 1: Changing Volume. When you’re asked to change volume you must respond in this formula id:0;vc where vc is the change to the volume. Here are some examples to what you may be asked and what you should respond with: User: turn volume up to 34, Response: id:0;34. User: raise my volume by 5, Response: id:0;+5. User: Decrease my volume by 5, Response: id:0;-5.User: Increase my volume, Response: id:0;+5. User: Decrease my volume, Response: id:0;-5.User: Volume 78, Response: id:0;78. When responding, your response should only be a completion to the formula, nothing else."},
      { role: "system", content: "Task 2: Playing and pausing audio. When asked to play or pause then respond with id:1 and nothing else. Examples: User: play, Response: id:1. User: pause, Response: id:1. User: play my audio, Response: id:1. User: Pause my audio, Response: id:1."},
      {role:"system", content: "Task 3: Command Line. You’ll have access to the users windows command line. Depending on what the user is asking for you’ll respond with command line commands that will be run. When it comes to sending commands you’ll use this formula id:2;cmn1 && cmd 2… with cmd1 being the 1st command, cmd2 being the 2nd command, and && being the and operator separating both commands. You can send as many commands as you need to send, just make sure to separate them by &&. You should first determine what the user wants and then split that up into multiple commands, then you should use the formula and split the commands by && accordingly. Examples: User: Open the Documents folder. Response: id:2;cd Documents User: Create a new folder called \"Project\" on the desktop. Response: id:2;cd desktop && mkdir Project User: Delete the file \"report.docx\" from the Downloads folder. Response: id:2;cd Downloads && del report.docx User: List all files in the current directory. Response: id:2;dir User: Copy \"image.jpg\" from the Pictures folder to the Documents folder. Response: id:2;cd Pictures && copy image.jpg C:\Users\Username\Documents User: Rename the file \"oldname.txt\" to \"newname.txt\" in the current directory. Response: id:2;ren oldname.txt newname.txt User: Change the directory to the Music folder. Response: id:2;cd Music. When responding, your response should only be a completion to the formula, nothing else."},
      { role: "user", content: requestData.message} 
    ],
  })

  res.status(200).send({
    bot: response.data.choices[0].message.content
  })
});

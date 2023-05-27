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
      { role: "system", content: " Here's the function to change volume: if(id == 0){int newVolume = int.Parse(toBeDone.Substring(5));CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;defaultPlaybackDevice.Volume = newVolume;}, the parameters for this are 1. the ID and 2. the desired volume. Your outputs should look something like this id:0;30, with the first number being the id too that function and the second number being the needed value for the action to be done. If you're asked to raise or lower the volume by an amount then add a + or - infront of the needed value, your response should look like this id:0;+30. If you're not given an exact amount to increase/decrease by then change it by 5. Your output MUST ALWAYS be in this format. Your output must not be outside of the id:0;xx formula"},
      {role: "system", content: " If you're asked to pause or play audio then respond with id:1 and nothing else. YOU MUST NOT RESPOND WITH ANYTHING BESIDES id:1"},
      { role: "user", content: requestData.message}
    ],
  })

  res.status(200).send({
    bot: response.data.choices[0].message.content
  })
});
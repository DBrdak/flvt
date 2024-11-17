import { Box, Typography, Accordion, AccordionSummary, AccordionDetails, Divider } from '@mui/material'
import ExpandMoreIcon from '@mui/icons-material/ExpandMore'
import { observer } from 'mobx-react-lite'

function Faq() {
    return (
        <Box sx={{ padding: 4, maxWidth: '800px', margin: '0 auto' }}>
            <Typography variant="h4" gutterBottom textAlign="center">
                About Flvt
            </Typography>
            <Divider sx={{ marginBottom: 4 }} />
            <Accordion>
                <AccordionSummary expandIcon={<ExpandMoreIcon />} aria-controls="panel1a-content" id="panel1a-header">
                    <Typography variant="h6">Why?</Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography>
                        We understand how challenging and time-consuming it can be to find that perfect apartment for rent.
                        With this in mind, we created a tool designed to streamline the process, making it faster and more efficient.
                        Our goal is to save you countless hours of searching and help you find your ideal apartment with ease.
                    </Typography>
                </AccordionDetails>
            </Accordion>
            <Accordion>
                <AccordionSummary expandIcon={<ExpandMoreIcon />} aria-controls="panel1a-content" id="panel1a-header">
                    <Typography variant="h6">What is this all about?</Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography>
                        Flvt is an advanced real estate search platform designed to simplify finding apartments for rent in supported cities.
                        By leveraging cutting-edge technology, including Large Language Models,
                        Flvt aggregates and processes data from various real estate advertisement portals to deliver highly tailored results.
                        With Flvt, you can save valuable time as it handles the tedious task of apartment hunting for you.
                        Simply relax and let Flvt match you with the perfect property, turning your search into an effortless experience.
                    </Typography>
                </AccordionDetails>
            </Accordion>
            <Accordion>
                <AccordionSummary expandIcon={<ExpandMoreIcon />} aria-controls="panel2a-content" id="panel2a-header">
                    <Typography variant="h6">How can I get started?</Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography>
                        Simply register and set up your first filter to get started with Flvt.
                        Once your filter is in place, you can instantly search through the available apartments in the system.
                        Additionally, Flvt keeps you updated by sending email notifications whenever new apartments matching your preferences become available,
                        ensuring you never miss an opportunity.
                    </Typography>
                </AccordionDetails>
            </Accordion>
            <Accordion>
                <AccordionSummary expandIcon={<ExpandMoreIcon />} aria-controls="panel3a-content" id="panel3a-header">
                    <Typography variant="h6">How to use it?</Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography>
                        Once you have set up your filter, you can browse advertisements using either the map view or list view.
                        Here are the key features of **Flvt** that you should know about to make the most of the platform:

                        <br />
                        <br />
                        <strong>• Follow: </strong>
                        This feature allows you to save advertisements for later, making it easy to access your favorite listings whenever you need them.

                        <br />
                        <br />
                        <strong>• Subscription Plans: </strong>
                        Currently, **Flvt** supports a Basic (free) subscription plan. In the future, additional subscription plans will be introduced, offering new features tailored to different needs.

                        <br />
                        <br />
                        <strong>• Filters: </strong>
                        Filters are applied periodically, meaning you may not see advertisements posted just a few minutes ago. However, this ensures you receive a manageable and relevant number of listings over time.
                        At the moment, only daily filtering is available, but more advanced filtering options are planned with the introduction of new subscription plans.

                        <br />
                        <br />
                        <strong>• Flags: </strong>
                        If you notice any discrepancies in the advertisements, you can use the flag feature to report them.
                        Once flagged, **Flvt**'s advanced processing tools will review and update the listing to ensure it meets accuracy standards.

                        <br />
                        <br />
                        <strong>• Browsing: </strong>
                        As you explore listings, **Flvt** keeps track of the advertisements you have viewed, whether by opening them on the original portal or interacting with them on the map.
                        This helps you monitor your activity and avoid revisiting already seen listings.
                    </Typography>
                </AccordionDetails>
            </Accordion>
            <Accordion>
                <AccordionSummary expandIcon={<ExpandMoreIcon />} aria-controls="panel4a-content" id="panel4a-header">
                    <Typography variant="h6">What can I expect from this product in the future?</Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography>
                        We are currently in the development phase, and at the moment, only the **Basic plan** is available.
                        This plan allows you to browse processed advertisements across the internet using a single filter.

                        <br />
                        Here are some **exciting features** planned for the future:

                        <br />
                        <br />
                        <strong>• Multiple Filters: </strong>
                        Support for multiple filters per user, giving you more flexibility in finding the perfect listings.

                        <br />
                        <br />
                        <strong>• AI-Powered Processes: </strong>
                        Exclusive AI-powered tools that can automatically browse advertisements and select the best ones based on your specific requirements.

                        <br />
                        <br />
                        <strong>• Price Evaluation Tools: </strong>
                        Tools to help you determine whether an apartment is priced fairly, ensuring you get the best value for your money.

                        <br />
                        <br />
                        <strong>• Service Expansion: </strong>
                        Expansion of our service to include more cities, making it easier for you to find the best apartments no matter where you live.

                        <br />
                        <br />
                        Stay tuned for these updates as we continue to enhance the platform!
                    </Typography>
                </AccordionDetails>
            </Accordion>

            <Box sx={{ textAlign: 'center', marginTop: 4 }}>
                <Typography variant="body2" color="text.secondary">
                    Developed with ❤️ by
                    <a
                        href="https://github.com/dbrdak"
                        target="_blank"
                        rel="noopener noreferrer"
                        style={{textDecoration: 'none', color: 'inherit', marginLeft: 4}}
                    >
                        dbrdak
                    </a>
                </Typography>
            </Box>
        </Box>
    )
}

export default observer(Faq)

import {observer} from "mobx-react-lite";
import {Form, Formik} from "formik";
import Box from "@mui/material/Box";
import FormLabel from "@mui/material/FormLabel";
import TextInput from "../sharedComponents/TextInput.tsx";
import Button from "@mui/material/Button";
import {AddBasicFilterBody} from "../../api/requestModels/addBasicFilter.ts";
import {useStore} from "../../stores/store.ts";
import * as Yup from 'yup'
import {availableCities} from "../../utils/constants/bussinessRules.ts";
import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";

function NewBasicFilterModal() {
    const {subscriberStore, modalStore} = useStore()

    const initialValues = {
        name: '',
        city: '',
        minPrice: '',
        maxPrice: '',
        minRooms: '',
        maxRooms: '',
        minArea: '',
        maxArea: ''
    }

    const handleSubmit = async (values: any) => {

        const sanitizedValues: AddBasicFilterBody = {
            ...values,
            minPrice: values.minPrice === '' ? null : Number(values.minPrice),
            maxPrice: values.maxPrice === '' ? null : Number(values.maxPrice),
            minRooms: values.minRooms === '' ? null : Number(values.minRooms),
            maxRooms: values.maxRooms === '' ? null : Number(values.maxRooms),
            minArea: values.minArea === '' ? null : Number(values.minArea),
            maxArea: values.maxArea === '' ? null : Number(values.maxArea),
        }

        const successful = await subscriberStore.addBasicFilterAsync(sanitizedValues)

        successful && modalStore.closeModal()
    }

    const validationSchema = Yup.object({
        name: Yup.string()
            .required("Name is required")
            .max(30, 'Name must be shorter than 30 characters'),
        city: Yup.string().required("City is required").oneOf(availableCities, 'Selected city is not supported'),
    })

    return (
        <Formik sx={{width: '100%'}} initialValues={initialValues} onSubmit={handleSubmit} validationSchema={validationSchema}>
            {({errors, values, setValues, isValid}) => (
                <Form style={{
                    width: '100%',
                    display: 'flex',
                    justifyContent: 'center',
                    gap: '2em',
                    alignItems: 'center',
                    flexDirection: 'column',
                    position: 'relative',
                }}>
                    <Box sx={{width: '100%'}}>
                        <FormLabel sx={{marginBottom: 1}}>Name</FormLabel>
                        <TextInput
                            placeholder={''}
                            type={'text'}
                            name={'name'}
                            fullwidth
                            errorMessage={isValid ? '' : errors.name}
                        />
                    </Box>
                    <Box sx={{width: '100%'}}>
                        <FormLabel htmlFor="email" sx={{marginBottom: 1}}>City</FormLabel>
                        <Select
                            fullWidth
                            label="City"
                            name={'city'}
                            value={values.city}
                            onChange={e => setValues({...values, city: e.target.value})}>
                            {availableCities.map((c, index) => (
                                <MenuItem key={index} value={c}>{c}</MenuItem>
                            ))}
                        </Select>
                    </Box>
                    <Box sx={{width: '100%'}}>
                        <FormLabel sx={{marginBottom: 1}}>Price</FormLabel>
                        <Box sx={{width: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
                            <TextInput
                                placeholder={''}
                                type={'number'}
                                name={'minPrice'}
                                fullwidth
                                errorMessage={errors.minPrice}
                            />
                            -
                            <TextInput
                                placeholder={''}
                                type={'number'}
                                name={'maxPrice'}
                                fullwidth
                                errorMessage={errors.maxPrice}
                            />
                        </Box>
                    </Box>
                    <Box sx={{width: '100%'}}>
                        <FormLabel sx={{marginBottom: 1}}>Rooms</FormLabel>
                        <Box sx={{width: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
                            <TextInput
                                placeholder={''}
                                type={'number'}
                                name={'minRooms'}
                                fullwidth
                                errorMessage={errors.minRooms}
                            />
                            -
                            <TextInput
                                placeholder={''}
                                type={'number'}
                                name={'maxRooms'}
                                fullwidth
                                errorMessage={errors.maxRooms}
                            />
                        </Box>
                    </Box>
                    <Box sx={{width: '100%'}}>
                        <FormLabel sx={{marginBottom: 1}}>Area</FormLabel>
                        <Box sx={{width: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
                            <TextInput
                                placeholder={''}
                                type={'number'}
                                name={'minArea'}
                                fullwidth
                                errorMessage={errors.minArea}
                            />
                            -
                            <TextInput
                                placeholder={''}
                                type={'number'}
                                name={'maxArea'}
                                fullwidth
                                errorMessage={errors.maxArea}
                            />
                        </Box>
                    </Box>
                    <Box width={'100%'}>
                        <Button type="submit" fullWidth variant="contained" color={'primary'} sx={{marginBottom: 2}}>
                            Add
                        </Button>
                        <Button fullWidth variant="outlined" color={'primary'} onClick={() => modalStore.closeModal()}>
                            Cancel
                        </Button>
                    </Box>
                </Form>
            )}
        </Formik>
    )
}

export default observer(NewBasicFilterModal);
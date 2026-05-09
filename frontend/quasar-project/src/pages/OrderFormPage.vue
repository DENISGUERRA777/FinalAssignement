<template>
	<q-page class="q-pa-md order-form-page">
		<!-- Header and primary actions -->
		<div class="row items-center q-col-gutter-md q-mb-md">
			<div class="col-12 col-md-5">
				<div class="text-h5 text-weight-bold">Order Management</div>
				<div class="text-caption text-grey-7">Create, update and delete orders with validated shipping address.</div>
			</div>

			<div class="col-12 col-md-4">
				<q-select
					dense
					outlined
					v-model="selectedOrderId"
					:options="orderOptions"
					label="Load existing order"
					emit-value
					map-options
					clearable
					@update:model-value="handleOrderSelection"
				/>
			</div>

			<div class="col-12 col-md-3">
				<div class="row justify-end q-gutter-sm">
					<q-btn outline color="primary" label="New" @click="resetForm" />
					<q-btn color="primary" label="Save" :loading="saving" :disable="!canSaveOrder" @click="saveOrder" />
					<q-btn
						flat
						color="negative"
						label="Delete"
						:disable="!form.orderId"
						:loading="deleting"
						@click="removeOrder"
					/>
				</div>
			</div>
		</div>

		<!-- Order details -->
		<q-card flat bordered class="q-mb-md">
			<q-card-section>
				<div class="text-subtitle1 text-weight-medium q-mb-sm">Order Information</div>
				<div class="row q-col-gutter-md">
					<div class="col-12 col-md-4">
						<q-input v-model="form.orderId" label="Order ID" dense outlined readonly />
					</div>
					<div class="col-12 col-md-4">
						<q-select
							v-model="form.customerId"
							label="Customer"
							dense
							outlined
							:options="customerOptions"
							emit-value
							map-options
							use-input
							fill-input
							required
							:rules="[(value) => !!value || 'Customer is required']"
						/>
					</div>
					<div class="col-12 col-md-4">
						<q-input v-model="form.orderDate" label="Order Date" type="date" dense outlined />
					</div>
					<div class="col-12 col-md-4">
						<q-select
							v-model="form.employeeId"
							label="Assigned Employee"
							dense
							outlined
							:options="employeeOptions"
							emit-value
							map-options
							required
							:rules="[(value) => value !== null && value !== undefined && value !== '' || 'Employee is required']"
						/>
					</div>
					<div class="col-12 col-md-4">
						<q-input v-model.number="form.shipVia" label="Ship Via" type="number" min="0" dense outlined />
					</div>
					<div class="col-12 col-md-4">
						<q-input v-model="form.requiredDate" label="Required Date" type="date" dense outlined />
					</div>
					<div class="col-12 col-md-4">
						<q-input v-model="form.shippedDate" label="Shipped Date" type="date" dense outlined />
					</div>
					<div class="col-12 col-md-4">
						<q-input v-model.number="form.freight" label="Freight" type="number" min="0" step="0.01" dense outlined />
					</div>
				</div>
			</q-card-section>
		</q-card>

		<!-- Line items -->
		<q-card flat bordered class="q-mb-md">
			<q-card-section>
				<div class="row items-center justify-between q-mb-sm">
					<div class="text-subtitle1 text-weight-medium">Line Items</div>
					<q-btn dense flat color="primary" icon="add" label="Add Line" @click="addLineItem" />
				</div>

				<div class="column q-gutter-sm">
					<div class="row q-col-gutter-sm text-caption text-grey-7">
						<div class="col-12 col-md-5">Product</div>
						<div class="col-6 col-md-2">Quantity</div>
						<div class="col-6 col-md-2">Unit Price</div>
						<div class="col-6 col-md-2">Discount</div>
						<div class="col-6 col-md-1 text-right">Actions</div>
					</div>

					<div
						v-for="(item, index) in form.orderDetails"
						:key="`line-${index}`"
						class="row q-col-gutter-sm items-center"
					>
						<div class="col-12 col-md-5">
							<q-select
								v-model="item.productId"
								dense
								outlined
								:options="productOptions"
								emit-value
								map-options
								label="Product"
								required
								:rules="[(value) => !!value || 'Select at least one product']"
								@update:model-value="(value) => applyProductDefaults(index, value)"
							/>
						</div>
						<div class="col-6 col-md-2">
							<q-input v-model.number="item.quantity" dense outlined type="number" min="1" label="Qty" />
						</div>
						<div class="col-6 col-md-2">
							<q-input v-model.number="item.unitPrice" dense outlined type="number" min="0" step="0.01" label="Unit" />
						</div>
						<div class="col-6 col-md-2">
							<q-input v-model.number="item.discount" dense outlined type="number" min="0" max="1" step="0.01" label="Disc" />
						</div>
						<div class="col-6 col-md-1 text-right">
							<q-btn
								flat
								dense
								round
								color="negative"
								icon="delete"
								:disable="form.orderDetails.length === 1"
								@click="removeLineItem(index)"
							/>
						</div>
					</div>
				</div>

				<div class="row justify-end q-mt-md">
					<div class="text-subtitle2 text-weight-medium">Order Total: {{ formatCurrency(orderTotal) }}</div>
				</div>
			</q-card-section>
		</q-card>

		<!-- Shipping address and validation -->
		<q-card flat bordered class="q-mb-md">
			<q-card-section>
				<div class="row items-center justify-between q-mb-sm">
					<div class="text-subtitle1 text-weight-medium">Shipping Address</div>
					<q-btn
						color="positive"
						outline
						icon="verified"
						label="Validate Address"
						:loading="validatingAddress"
						@click="validateCurrentAddress"
					/>
				</div>

				<div class="row q-col-gutter-md">
					<div class="col-12 col-md-6">
						<q-input v-model="form.shipName" label="Ship Name" dense outlined />
					</div>
					<div class="col-12 col-md-6">
						<q-input v-model="form.shipAddress" label="Street" dense outlined />
					</div>
					<div class="col-12 col-md-4">
						<q-input v-model="form.shipCity" label="City" dense outlined />
					</div>
					<div class="col-12 col-md-4">
						<q-input v-model="form.shipRegion" label="State / Region" dense outlined />
					</div>
					<div class="col-12 col-md-4">
						<q-input v-model="form.shipPostalCode" label="Postal Code" dense outlined />
					</div>
					<div class="col-12 col-md-6">
						<q-input v-model="form.shipCountry" label="Country (ISO code preferred)" dense outlined />
					</div>
					<div class="col-12 col-md-3">
						<q-input :model-value="formattedLatitude" label="Latitude" dense outlined readonly />
					</div>
					<div class="col-12 col-md-3">
						<q-input :model-value="formattedLongitude" label="Longitude" dense outlined readonly />
					</div>
				</div>

				<div v-if="validatedAddress.formattedAddress" class="q-mt-md validated-address">
					<div class="text-caption text-grey-7">Validated address</div>
					<div class="text-body2 text-weight-medium">{{ validatedAddress.formattedAddress }}</div>
					<div class="text-caption text-grey-7 q-mt-xs">Verdict: {{ validatedAddress.verdict || 'Unknown' }}</div>
				</div>
			</q-card-section>
		</q-card>

		<!-- Map preview -->
		<q-card flat bordered>
			<q-card-section>
				<div class="text-subtitle1 text-weight-medium q-mb-sm">Map Preview</div>
				<div class="map-shell">
					<iframe
						v-if="mapEmbedUrl"
						:src="mapEmbedUrl"
						class="map-frame"
						loading="lazy"
						referrerpolicy="no-referrer-when-downgrade"
						allowfullscreen
					></iframe>
					<div v-else class="map-placeholder text-grey-7">
						Validate an address to show it on the map.
					</div>
				</div>
			</q-card-section>
		</q-card>
	</q-page>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute } from 'vue-router'
import { createOrder, deleteOrder, getOrders, updateOrder, searchOrders } from 'src/services/ordersApi'
import { getCustomers } from 'src/services/CustomersApi'
import { getProducts } from 'src/services/ProductsApi'
import { validateAddress } from 'src/services/GoogleValidationApi'

const $q = useQuasar()
const route = useRoute()

// Loaded reference data
const orders = ref([])
const customers = ref([])
const products = ref([])

// Form and UI state
const selectedOrderId = ref(null)
const saving = ref(false)
const deleting = ref(false)
const validatingAddress = ref(false)

const validatedAddress = ref({
	formattedAddress: '',
	latitude: null,
	longitude: null,
	verdict: '',
})

const form = ref(buildEmptyForm())

// Dropdown options
const customerOptions = computed(() => {
	return customers.value.map((customer) => ({
		label: `${customer.companyName || customer.contactName || customer.customerId} (${customer.customerId})`,
		value: customer.customerId,
	}))
})

const productOptions = computed(() => {
	return products.value.map((product) => ({
		label: `${product.productName} (#${product.productId})`,
		value: product.productId,
	}))
})

const employeeOptions = computed(() => {
	const fromOrders = Array.from(
		new Set(
			orders.value
				.map((order) => Number(order.employeeId))
				.filter((id) => Number.isFinite(id) && id > 0),
		),
	)

	const fallback = [1, 2, 3, 4, 5]
	const ids = fromOrders.length ? fromOrders.sort((a, b) => a - b) : fallback

	return ids.map((id) => ({
		label: `Employee #${id}`,
		value: id,
	}))
})

const orderOptions = computed(() => {
	return orders.value
		.slice()
		.sort((a, b) => Number(b.orderId) - Number(a.orderId))
		.map((order) => ({
			label: `#${order.orderId} - ${order.customerId || order.shipName || 'Unknown customer'}`,
			value: order.orderId,
		}))
})

const orderTotal = computed(() => {
	const detailsTotal = form.value.orderDetails.reduce((sum, detail) => {
		const quantity = safeNumber(detail.quantity)
		const unitPrice = safeNumber(detail.unitPrice)
		const discount = safeNumber(detail.discount)
		return sum + quantity * unitPrice * (1 - discount)
	}, 0)

	return detailsTotal + safeNumber(form.value.freight)
})

const canSaveOrder = computed(() => {
	const hasCustomer = Boolean(form.value.customerId)
	const hasEmployee = form.value.employeeId !== null && form.value.employeeId !== undefined && form.value.employeeId !== ''
	const hasProduct = form.value.orderDetails.some((detail) => Boolean(detail.productId))

	return hasCustomer && hasEmployee && hasProduct
})

const formattedLatitude = computed(() => {
	return validatedAddress.value.latitude == null ? '-' : validatedAddress.value.latitude.toFixed(6)
})

const formattedLongitude = computed(() => {
	return validatedAddress.value.longitude == null ? '-' : validatedAddress.value.longitude.toFixed(6)
})

const mapEmbedUrl = computed(() => {
	if (validatedAddress.value.latitude != null && validatedAddress.value.longitude != null) {
		return `https://maps.google.com/maps?q=${validatedAddress.value.latitude},${validatedAddress.value.longitude}&z=15&output=embed`
	}

	if (validatedAddress.value.formattedAddress) {
		return `https://maps.google.com/maps?q=${encodeURIComponent(validatedAddress.value.formattedAddress)}&z=14&output=embed`
	}

	return ''
})

let isProgrammaticAddressChange = false

// Reset validated address whenever the user edits the shipping fields manually
watch(
	() => [form.value.shipAddress, form.value.shipCity, form.value.shipRegion, form.value.shipPostalCode, form.value.shipCountry],
	() => {
		if (isProgrammaticAddressChange) {
			return
		}
		
		validatedAddress.value = {
			formattedAddress: '',
			latitude: null,
			longitude: null,
			verdict: '',
		}
	},
)

// Initial data load
onMounted(async () => {
	await loadInitialData()
	await syncOrderFromRoute()
})

// API data loading helpers
async function loadInitialData() {
	try {
		const [ordersResponse, customersResponse, productsResponse] = await Promise.all([
			getOrders({ page: 1, pageSize: 400 }),
			getCustomers({ page: 1, pageSize: 400 }),
			getProducts({ page: 1, pageSize: 400 }),
		])

		orders.value = normalizeItems(ordersResponse)
		customers.value = normalizeItems(customersResponse)
		products.value = normalizeItems(productsResponse)
	} catch (error) {
		notifyError(error, 'Could not load order form data')
	}
}

function normalizeItems(response) {
	if (Array.isArray(response)) {
		return response
	}

	if (Array.isArray(response?.items)) {
		return response.items
	}

	return []
}

// Form builders
function buildEmptyForm() {
	const today = new Date().toISOString().slice(0, 10)
	return {
		orderId: null,
		customerId: null,
		employeeId: null,
		orderDate: today,
		requiredDate: '',
		shippedDate: '',
		shipVia: null,
		freight: 0,
		shipName: '',
		shipAddress: '',
		shipCity: '',
		shipRegion: '',
		shipPostalCode: '',
		shipCountry: '',
		orderDetails: [buildEmptyLineItem()],
	}
}

function buildEmptyLineItem() {
	return {
		productId: null,
		quantity: 1,
		unitPrice: 0,
		discount: 0,
	}
}

// Line item actions
function addLineItem() {
	form.value.orderDetails.push(buildEmptyLineItem())
}

function removeLineItem(index) {
	if (form.value.orderDetails.length === 1) {
		return
	}

	form.value.orderDetails.splice(index, 1)
}

function applyProductDefaults(index, productId) {
	const product = products.value.find((item) => Number(item.productId) === Number(productId))
	if (!product) {
		return
	}

	const line = form.value.orderDetails[index]
	if (!line) {
		return
	}

	line.unitPrice = safeNumber(product.unitPrice)
}

// Existing order selection
function handleOrderSelection(orderId) {
	if (!orderId) {
		return
	}

	const selected = orders.value.find((order) => Number(order.orderId) === Number(orderId))
	if (selected) {
		loadOrderInForm(selected)
	}
}

function loadOrderInForm(order) {
	form.value = {
		orderId: order.orderId,
		customerId: order.customerId || null,
		employeeId: order.employeeId ?? null,
		orderDate: toDateInput(order.orderDate),
		requiredDate: toDateInput(order.requiredDate),
		shippedDate: toDateInput(order.shippedDate),
		shipVia: order.shipVia ?? null,
		freight: safeNumber(order.freight),
		shipName: order.shipName || '',
		shipAddress: order.shipAddress || '',
		shipCity: order.shipCity || '',
		shipRegion: order.shipRegion || '',
		shipPostalCode: order.shipPostalCode || '',
		shipCountry: order.shipCountry || '',
		orderDetails: Array.isArray(order.orderDetails) && order.orderDetails.length
			? order.orderDetails.map((detail) => ({
				productId: detail.productId,
				quantity: safeNumber(detail.quantity) || 1,
				unitPrice: safeNumber(detail.unitPrice),
				discount: safeNumber(detail.discount),
			}))
			: [buildEmptyLineItem()],
	}

	validatedAddress.value = {
		formattedAddress: order.shipAddress || '',
		latitude: null,
		longitude: null,
		verdict: '',
	}
}

// Address validation and map updates
async function validateCurrentAddress() {
	const queryParts = [
		form.value.shipAddress,
		form.value.shipCity,
		form.value.shipRegion,
		form.value.shipPostalCode,
		form.value.shipCountry,
	].filter(Boolean)

	if (!queryParts.length) {
		$q.notify({ type: 'warning', message: 'Enter a shipping address before validation.' })
		return
	}

	validatingAddress.value = true

	try {
		const response = await validateAddress({
			address: queryParts.join(', '),
			// Avoid forcing regionCode to prevent "Unsupported region code" 400 errors for places like VE
			languageCode: 'en',
		})

		const result = response?.result
		const postal = result?.address?.postalAddress ?? {}
		const location = result?.geocode?.location ?? {}

		isProgrammaticAddressChange = true

		form.value.shipAddress = Array.isArray(postal.addressLines) && postal.addressLines.length
			? postal.addressLines.join(', ')
			: form.value.shipAddress
		form.value.shipCity = postal.locality || form.value.shipCity
		form.value.shipRegion = postal.administrativeArea || form.value.shipRegion
		form.value.shipPostalCode = postal.postalCode || form.value.shipPostalCode
		form.value.shipCountry = postal.regionCode || form.value.shipCountry

		validatedAddress.value = {
			formattedAddress: result?.address?.formattedAddress || queryParts.join(', '),
			latitude: typeof location.latitude === 'number' ? location.latitude : (typeof location.lat === 'number' ? location.lat : null),
			longitude: typeof location.longitude === 'number' ? location.longitude : (typeof location.lng === 'number' ? location.lng : null),
			verdict: result?.verdict?.possibleNextAction || '',
		}

		$q.notify({ type: 'positive', message: 'Address validated successfully.' })
		
		setTimeout(() => {
			isProgrammaticAddressChange = false
		}, 50)
	} catch (error) {
		notifyError(error, 'Address validation failed')
	} finally {
		validatingAddress.value = false
	}
}

// Save and delete actions
async function saveOrder() {
	if (!form.value.customerId) {
		$q.notify({ type: 'warning', message: 'Customer is required.' })
		return
	}

	if (form.value.employeeId === null || form.value.employeeId === undefined || form.value.employeeId === '') {
		$q.notify({ type: 'warning', message: 'Employee is required.' })
		return
	}

	const orderDetails = form.value.orderDetails
		.filter((detail) => detail.productId)
		.map((detail) => ({
			orderId: safeNumber(form.value.orderId),
			productId: safeNumber(detail.productId),
			unitPrice: safeNumber(detail.unitPrice),
			quantity: Math.max(1, Math.round(safeNumber(detail.quantity))),
			discount: safeNumber(detail.discount),
		}))

	if (!orderDetails.length) {
		$q.notify({ type: 'warning', message: 'Add at least one product line.' })
		return
	}

	const payload = {
		orderId: safeNumber(form.value.orderId),
		customerId: form.value.customerId,
		employeeId: toNullIfEmpty(form.value.employeeId),
		orderDate: toIsoOrNull(form.value.orderDate),
		requiredDate: toIsoOrNull(form.value.requiredDate),
		shippedDate: toIsoOrNull(form.value.shippedDate),
		shipVia: toNullIfEmpty(form.value.shipVia),
		freight: toNullIfEmpty(form.value.freight),
		shipName: toNullIfEmpty(form.value.shipName),
		shipAddress: toNullIfEmpty(validatedAddress.value.formattedAddress || form.value.shipAddress),
		shipCity: toNullIfEmpty(form.value.shipCity),
		shipRegion: toNullIfEmpty(form.value.shipRegion),
		shipPostalCode: toNullIfEmpty(form.value.shipPostalCode),
		shipCountry: toNullIfEmpty(form.value.shipCountry),
		orderDetails,
	}

	saving.value = true

	try {
		const isUpdate = Boolean(form.value.orderId)
		const result = isUpdate
			? await updateOrder(form.value.orderId, payload)
			: await createOrder(payload)

		showResultDialog('Success', `<p>${isUpdate ? 'Order updated' : 'Order created'} successfully.</p><br/><code>${formatApiResponse(result)}</code>`)

		await refreshOrders()

		if (result?.orderId) {
			selectedOrderId.value = result.orderId
			const current = orders.value.find((order) => Number(order.orderId) === Number(result.orderId))
			if (current) {
				loadOrderInForm(current)
			}
		}
	} catch (error) {
		showResultDialog('Error', error instanceof Error ? error.message : 'Could not save order')
	} finally {
		saving.value = false
	}
}

async function removeOrder() {
	if (!form.value.orderId) {
		return
	}

	const confirmed = window.confirm(`Delete order #${form.value.orderId}?`)
	if (!confirmed) {
		return
	}

	deleting.value = true

	try {
		await deleteOrder(form.value.orderId)
		$q.notify({ type: 'positive', message: 'Order deleted.' })
		resetForm()
		await refreshOrders()
	} catch (error) {
		notifyError(error, 'Could not delete order')
	} finally {
		deleting.value = false
	}
}

function resetForm() {
	selectedOrderId.value = null
	form.value = buildEmptyForm()
	validatedAddress.value = {
		formattedAddress: '',
		latitude: null,
		longitude: null,
		verdict: '',
	}
}

async function refreshOrders() {
	const response = await getOrders({ page: 1, pageSize: 400 })
	orders.value = normalizeItems(response)
}

async function syncOrderFromRoute() {
	const routeOrderId = route.query.orderId
	if (!routeOrderId) {
		return
	}

	let selected = orders.value.find((order) => String(order.orderId) === String(routeOrderId))
	
	if (!selected) {
		try {
			const searchResult = await searchOrders({ orderId: routeOrderId })
			if (searchResult && searchResult.length > 0) {
				selected = searchResult[0]
			}
		} catch (error) {
			console.error('Could not fetch order from route', error)
		}
	}

	if (!selected) {
		return
	}

	selectedOrderId.value = selected.orderId
	loadOrderInForm(selected)
}

// Data conversion helpers
function toDateInput(value) {
	if (!value) {
		return ''
	}

	const date = new Date(value)
	if (Number.isNaN(date.getTime())) {
		return ''
	}

	return date.toISOString().slice(0, 10)
}

function toIsoOrNull(value) {
	if (!value) {
		return null
	}

	const date = new Date(value)
	return Number.isNaN(date.getTime()) ? null : date.toISOString()
}

function safeNumber(value) {
	const n = Number(value)
	return Number.isFinite(n) ? n : 0
}

function toNullIfEmpty(value) {
	if (value === null || value === undefined || value === '') {
		return null
	}

	if (typeof value === 'string') {
		const trimmed = value.trim()
		return trimmed === '' ? null : trimmed
	}

	if (typeof value === 'number') {
		return Number.isFinite(value) ? value : null
	}

	return value
}

// Formatting and notification helpers
function formatCurrency(value) {
	return new Intl.NumberFormat('en-US', {
		style: 'currency',
		currency: 'USD',
	}).format(safeNumber(value))
}

function formatApiResponse(order) {
	if (!order || typeof order !== 'object') {
		return 'No response data returned.'
	}

	const summary = {
		orderId: order.orderId ?? null,
		customerId: order.customerId ?? null,
		employeeId: order.employeeId ?? null,
		shipVia: order.shipVia ?? null,
		freight: order.freight ?? null,
		orderDetails: Array.isArray(order.orderDetails) ? order.orderDetails.length : 0,
	}

	return Object.entries(summary)
		.map(([key, value]) => `${key}: ${value}`)
		.join(' | ')
}

function showResultDialog(title, message) {
	$q.dialog({
		title,
		message,
		html: true,
		ok: {
			label: 'OK',
			color: title === 'Error' ? 'negative' : 'primary',
		},
		persistent: true,
	})
}

function notifyError(error, fallbackMessage) {
	const message = error instanceof Error ? error.message : fallbackMessage
	$q.notify({
		type: 'negative',
		message,
	})
}
</script>

<style scoped>
/* Page background */
.order-form-page {
	background: linear-gradient(180deg, #f8fafc 0%, #eef2ff 100%);
}

/* Validated address callout */
.validated-address {
	border: 1px solid rgba(34, 197, 94, 0.32);
	border-radius: 10px;
	background: rgba(34, 197, 94, 0.06);
	padding: 10px 12px;
}

/* Map container */
.map-shell {
	border: 1px solid rgba(148, 163, 184, 0.35);
	border-radius: 12px;
	overflow: hidden;
	min-height: 320px;
	background: #e2e8f0;
}

.map-frame {
	display: block;
	width: 100%;
	height: 380px;
	border: 0;
}

.map-placeholder {
	min-height: 320px;
	display: flex;
	align-items: center;
	justify-content: center;
	text-align: center;
	padding: 16px;
}
</style>
